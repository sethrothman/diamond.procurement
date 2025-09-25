using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraSplashScreen;
using Diamond.Procurement.Data.Contracts;
using Diamond.Procurement.Domain.Util;
using Diamond.Procurement.Win.Helpers;
using Diamond.Procurement.Win.Services;
using DocumentFormat.OpenXml.EMMA;
using System.Globalization;
using System.IO;
using System.Net;

namespace Diamond.Procurement.Win.UserControls
{
    public partial class ImportPage : XtraUserControl
    {
        private readonly Dictionary<FileKind, StepImportSlot> _steps;
        private readonly IUiImportOrchestrator _orchestrator;
        private readonly IImportStatusRepository _statusRepo;
        private readonly IFtpEndpointRepository _ftpRepo;
        private FileKind? _ftpFetchTargetKind;

        private readonly ToolTipController _ttc = new();
        private IReadOnlyList<BuyerInventoryLastUpdate> _buyerInvDetails = Array.Empty<BuyerInventoryLastUpdate>();

        public ImportPage(IUiImportOrchestrator orchestrator, IImportStatusRepository statusRepo, IFtpEndpointRepository ftpRepo)
        {
            InitializeComponent();

            stepProgressBar1.SelectedItemIndex = -1;

            _orchestrator = orchestrator;
            _statusRepo = statusRepo;
            _ftpRepo = ftpRepo;

            stepProgressBar1.ToolTipController = _ttc;
            _ttc.GetActiveObjectInfo += ToolTipController_GetActiveObjectInfo;

            AllowDrop = true;
            DragEnter += (s, e) => { if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true) e.Effect = DragDropEffects.Copy; };
            DragDrop += (s, e) => AssignFiles((string[])e.Data!.GetData(DataFormats.FileDrop)!);

            _steps = new()
            {
                { FileKind.BuyerInventory, new StepImportSlot(FileKind.BuyerInventory, stepBuyerInventory) },
                { FileKind.BuyerForecast, new StepImportSlot(FileKind.BuyerForecast, stepBuyerForecast) },
                { FileKind.VendorForecast, new StepImportSlot(FileKind.VendorForecast, stepVendorForecast) },
                { FileKind.MainframeInventory, new StepImportSlot(FileKind.MainframeInventory, stepMainframe) },
                { FileKind.UpcComp, new StepImportSlot(FileKind.UpcComp, stepUpcComp) },
            };

            foreach (var s in _steps.Values)
            {
                s.IconQueued = svgImageCollection1["notstarted"]; // queued/file icon
                s.IconWorking = svgImageCollection1["bo_statemachine"]; // optional
                s.IconSuccess = svgImageCollection1["actions_checkcircled"]; // checkmark/OK
                s.IconError = svgImageCollection1["bo_attention"]; // cross/error
                s.IconSkipped = svgImageCollection1["actions_forbid"]; // subtle dash
            }

            btnProcess.Enabled = false;
            UpdateProcessButtonState();

            this.stepProgressBar1.MouseUp += stepProgressBar1_MouseUp;
            bbiFetch.ItemClick += miFetchFromFtp_Click;

            labelControl1.Text = $"• Make sure the filename of the Buyer Inventory file contains its \"effective date\" to ensure proper week number calculations (i.e. L&&R Buyer Inventory <color=red><b>{DateTime.Today:yyyyMMdd}</b></color>.xlsx)";

            // Load once when the control is created/shown
            this.Load += async (_, __) => await UpdateStepDatesAsync();
        }

        private void ToolTipController_GetActiveObjectInfo(object? sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            // Convert global mouse pos -> stepProgressBar1 client pos
            var screenPos = Control.MousePosition;
            Point clientPos = stepProgressBar1.PointToClient(screenPos);

            // If the mouse isn't actually over stepProgressBar1, bail
            if (!new Rectangle(Point.Empty, stepProgressBar1.Size).Contains(clientPos))
                return;

            // Do a proper hit-test on the control itself
            StepProgressBarHitInfo hit = stepProgressBar1.CalcHitInfo(clientPos);
            if (!hit.InItem || !ReferenceEquals(hit.Item, stepBuyerInventory) || _buyerInvDetails.Count == 0)
                return;

            if (hit.Item == stepBuyerInventory && _buyerInvDetails.Count > 0)
            {
                // Build the multi-line tooltip
                var stt = new SuperToolTip();
                stt.Items.Add(new ToolTipTitleItem { Text = "Buyer Inventory – Last Imports" });

                var lines = _buyerInvDetails
                    .OrderBy(d => d.BuyerName)
                    .Select(d => $"<font='Consolas' size=10><b>{d.BuyerName}</b>: {d.LocalTime:MMM d, h:mm tt}")
                    .ToArray();
                stt.Items.Add(new ToolTipItem { Text = string.Join(Environment.NewLine, lines), AllowHtmlText = DefaultBoolean.True });
                
                // Use the item as the identity and position the tooltip at the cursor
                e.Info = new ToolTipControlInfo(hit.Item, "") { SuperTip = stt };
            }
        }

        private async void miFetchFromFtp_Click(object? sender, EventArgs e)
        {
            await Helpers.OverlayHelper.RunAsync(this, async () =>
            {
                try
                {
                    var target = _ftpFetchTargetKind ?? FileKind.MainframeInventory;
                    AppendLog($"Fetching {(target == FileKind.UpcComp ? "UPC Comp" : "Mainframe Inventory")} from FTP…");

                    var ep = await _ftpRepo.GetActiveAsync();
                    if (ep is null)
                    {
                        AppendLog("No active FTP endpoint configured. Check dbo.FtpEndpoint.");
                        XtraMessageBox.Show(this, "No active FTP endpoint configured. Check dbo.FtpEndpoint.", "FTP Settings", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // For UPC Comp, override the filename on-the-fly
                    if (target == FileKind.UpcComp)
                    {
                        ep = new FtpEndpoint
                        {
                            FtpEndpointId = ep.FtpEndpointId,
                            FtpAddress = ep.FtpAddress,
                            FtpPort = ep.FtpPort,
                            FtpPath = ep.FtpPath,
                            FileName = "CMPT_RPT_UPC.CSV",   // fixed name per spec
                            User = ep.User,
                            Password = ep.Password,
                            IsActive = ep.IsActive,
                            LastTested = ep.LastTested
                        };
                    }

                    var progress = new Progress<string>(s => AppendLog(s));
                    using var cts = new CancellationTokenSource();

                    var localPath = await FtpDownloader.DownloadAsync(ep, progress, cts.Token);

                    // Reuse drag & drop flow (inference + queue)
                    AssignFiles(new[] { localPath });

                    AppendLog("File fetched and queued successfully.");
                }
                catch (Exception ex)
                {
                    AppendLog($"FTP fetch failed: {ex.Message}");
                    XtraMessageBox.Show(this, ex.Message, "FTP Download Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        // NEW: pull last-update dates from repo and push into slots (date-only)
        private async Task UpdateStepDatesAsync(CancellationToken ct = default)
        {
            // Rollup for the 5 steps (unchanged)
            var updates = await _statusRepo.GetLastUpdatesAsync(ct);

            // NEW: detailed Buyer Inventory rows (per-buyer SysStartTime → local)
            _buyerInvDetails = await _statusRepo.GetBuyerInventoryLastUpdatesAsync(ct);

            // Buyer Inventory: show a compact rollup in the step, details in the tooltip
            if (_buyerInvDetails.Count > 0)
            {
                var mostRecentLocal = _buyerInvDetails.Max(d => d.LocalTime);
                var count = _buyerInvDetails.Count;

                // Put a short rollup in the content block (keeps layout tidy)
                stepBuyerInventory.ContentBlock2.Caption = $"{count}/{count} received";
                //stepBuyerInventory.ContentBlock2.Description = $"as of {mostRecentLocal:MMM d, h:mm tt}";

                // Also keep your existing one-line date on the slot (if you use it elsewhere)
                _steps[FileKind.BuyerInventory].SetLastImport(DateOnly.FromDateTime(mostRecentLocal));
            }
            else
            {
                stepBuyerInventory.ContentBlock2.Caption = "No recent imports";
                //stepBuyerInventory.ContentBlock2.Description = string.Empty;
                _steps[FileKind.BuyerInventory].SetLastImport(null);
            }

            // Other four steps: same as before
            _steps[FileKind.BuyerForecast].SetLastImport(updates.BuyerForecast);
            _steps[FileKind.VendorForecast].SetLastImport(updates.VendorForecast);
            _steps[FileKind.MainframeInventory].SetLastImport(updates.MainframeInventory);
            _steps[FileKind.UpcComp].SetLastImport(updates.UpcComp);
        }

        private void AssignFiles(IEnumerable<string> paths)
        {
            Cursor = Cursors.WaitCursor;

            foreach (var path in paths)
            {
                string fileName = Path.GetFileName(path);
                var info = FileInference.TryDetectInfo(path);

                if (info is null)
                {
                    AppendLog($"Dropped: {fileName} → Unrecognized file; skipped.");
                    continue;
                }


                var kind = info.Value.Kind;
                var slot = _steps[kind];

                // Log the friendly type right away
                var display = FileInference.ToDisplay(info.Value);
                AppendLog($"Dropped: {Path.GetFileName(path)} → {display}");

                slot.AssignFile(path, svgImageCollection1["grandtotalsoncolumnsonlypivottable"]);
                slot.SignatureMap = info?.SignatureMap; // stash the map on the slot

                if (slot.PartyKind == FileInference.PartyKind.Buyer)
                {
                    // use inferred id if present; otherwise recall; otherwise leave null (prompt later if you prefer)
                    var inferred = info.Value.PartyId;
                    var recalled = PartyMemory.RecallBuyer(slot.FilePath!);
                    slot.PartyId = inferred ?? recalled;

                    // remember only if we actually have an id
                    if (slot.PartyId.HasValue)
                        PartyMemory.RememberBuyer(slot.FilePath!, slot.PartyId.Value);
                }
                else if (slot.PartyKind == FileInference.PartyKind.Vendor)
                {
                    // today only one vendor – set a default if you like, or leave null until you add detection
                    slot.PartyId = 1;
                }
            }

            UpdateProcessButtonState();
            Cursor = Cursors.Default;
        }

        private void UpdateProcessButtonState()
        {
            btnProcess.Enabled = _steps.Values.Count(x => !string.IsNullOrEmpty(x.FilePath)) > 0;
        }

        private void AppendLog(string msg)
        {
            if (InvokeRequired) { BeginInvoke(new Action<string>(AppendLog), msg); return; }
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}{Environment.NewLine}");
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
            txtLog.Refresh(); // ensure we see updates during long work
        }

        private async void btnProcess_Click(object sender, EventArgs e)
        {
            btnProcess.Enabled = false;

            await Helpers.OverlayHelper.RunAsync(this, async () =>
            {
                try
                {
                    // mark assigned steps as "processing" & unassigned as "skipped"
                    foreach (var s in _steps.Values.Where(s => s.FilePath == null))
                        s.MarkSkipped();

                    foreach (var s in _steps.Values.Where(s => s.FilePath != null))
                        s.MarkProcessing();

                    var files = _steps.ToDictionary(k => k.Key, v => v.Value.FilePath!);

                    var progress = new Progress<ImportProgress>(p =>
                    {
                        var slot = _steps[p.Kind];
                        if (!p.IsDone)
                        {
                            slot.MarkProcessing("Processing…");
                            AppendLog(p.Message);
                        }
                        else if (p.IsSuccess)
                        {
                            slot.MarkSuccess(DateTime.Now);         // short, success-y
                            AppendLog(p.Message);                   // details go to the log
                        }
                        else
                        {
                            slot.MarkError("Import failed");        // concise in the step
                            AppendLog(p.Message);                   // details to the log
                        }
                    });

                    using var cts = new CancellationTokenSource();
                    await _orchestrator.RunAllAsync(_steps, progress, cts.Token);

                    // Optionally re-pull persisted dates:
                    await UpdateStepDatesAsync();
                }
                catch (Exception ex)
                {
                    foreach (var s in _steps.Values.Where(s => s.FilePath != null))
                    {
                        s.MarkError("Unhandled error");
                        AppendLog(ex.Message);
                    }
                }
                finally
                {
                    UpdateProcessButtonState();
                    stepProgressBar1.SelectedItemIndex = -1; // keep the bar from picking the first item
                }
            });
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            foreach (var s in _steps.Values)
                s.ClearAssigned();

            await UpdateStepDatesAsync();
        }

        // === Context menu behavior only over the "Mainframe Inventory" item ===
        private void stepProgressBar1_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            var hit = stepProgressBar1.CalcHitInfo(e.Location);
            if (hit.Item == stepMainframe)
                _ftpFetchTargetKind = FileKind.MainframeInventory;
            else if (hit.Item == stepUpcComp)
                _ftpFetchTargetKind = FileKind.UpcComp;
            else
                return;

            Point screenPoint = stepProgressBar1.PointToScreen(e.Location);
            popupMenu1.ShowPopup(screenPoint);
        }
    }
}
