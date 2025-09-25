using DevExpress.XtraEditors;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using Diamond.Procurement.Data;
using Diamond.Procurement.Domain.Enums;
using Diamond.Procurement.Domain.Models;
using Diamond.Procurement.Win.GridSchemas;
using Diamond.Procurement.Win.Helpers;
using Diamond.Procurement.Win.ViewModels;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Diamond.Procurement.Win.UserControls
{
    public interface IDirtyAware
    {
        bool IsDirty { get; }
        /// <summary>Prompts the user to save/discard/cancel. Return true to continue closing, false to cancel.</summary>
        Task<bool> PromptToSaveIfDirtyAsync(IWin32Window owner);
    }

    public partial class MasterListAnalysisPage : XtraUserControl, IDirtyAware
    {
        private readonly IListTypeGridSchemaFactory _schemaFactory = new ListTypeGridSchemaFactory();
        private AdvBandedGridView? _bv;

        private GridSettingsPersistence _gridPersistence = new GridSettingsPersistence();

        // Add with your other private fields
        private readonly MenuSpecFactory _menuSpecFactory = new();
        private IListTypeMenuSpecProvider? _menuSpecProvider;

        // Injected later via Configure(...)
        private IOrderVendorRepository? _repo;
        private MasterListRepository? _masterListRepository;

        // Page context
        private int _orderVendorId;
        private DateTime? _forecastThruDate;
        private DateTime? _forecastFromDate;
        private int _targetWeeksOfInv;
        private int _masterListId;
        private bool _reactiveUpdatesEnabled;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ListTypeId CurrentListTypeId { get; private set; } = (ListTypeId)0;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CurrentBuyerId { get; private set; } = (int)0;

        // Replace your MasterListId property with this:
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MasterListId
        {
            get => _masterListId;
            set
            {
                if (_masterListId == value) return;
                var old = _masterListId;
                _masterListId = value;

                if (!_reactiveUpdatesEnabled) return; // ignore during initial load

                // fire and forget (UI context)
                _ = OnMasterListChangedAsync(old, value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int OrderVendorId
        {
            get => _orderVendorId;
            set => _orderVendorId = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime? ForecastFromDate
        {
            get => _forecastFromDate;
            set => _forecastFromDate = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime? ForecastThruDate
        {
            get => _forecastThruDate;
            set => _forecastThruDate = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TargetWeeksOfInv
        {
            get => _targetWeeksOfInv;
            set => _targetWeeksOfInv = value;
        }

        public bool IsDirty => _rows.Any(r => r.IsDirty);

        // Binding
        private readonly BindingList<VendorOrderRowVM> _rows = [];

        private bool _wired;

        public MasterListAnalysisPage()
        {
            InitializeComponent(); // Designer-generated

            // Null-safe bindings (with defaults)
            lueMasterList.DataBindings.Add(
                "EditValue", this, nameof(MasterListId),
                true, DataSourceUpdateMode.OnPropertyChanged, 0);

            lueOrderMonth.DataBindings.Add(
                "EditValue", this, nameof(OrderVendorId),
                true, DataSourceUpdateMode.OnPropertyChanged, 0);

            dtForecastFrom.DataBindings.Add(
                "DateTime", this, nameof(ForecastFromDate),
                true, DataSourceUpdateMode.OnPropertyChanged, null);

            dtForecastDate.DataBindings.Add(
                "DateTime", this, nameof(ForecastThruDate),
                true, DataSourceUpdateMode.OnPropertyChanged, null);

            seTargetWeeks.DataBindings.Add(
                "EditValue", this, nameof(TargetWeeksOfInv),
                true, DataSourceUpdateMode.OnPropertyChanged, 0);

            // Lightweight, Designer-safe setup:
            gridControl1.DataSource = _rows;
        }

        /// <summary>
        /// Designer-safe “injection”: call this right after you new up the control.
        /// </summary>
        public void Configure(IOrderVendorRepository repo, MasterListRepository masterListRepository) //, int vendorId, DateTime forecastThruDate, DateTime defaultMonth)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _masterListRepository = masterListRepository ?? throw new ArgumentNullException(nameof(_masterListRepository));
        }

        /// <summary>
        /// Call this from Navigate(...) after Configure(...). Loads months + grid.
        /// </summary>
        public async Task InitializeAsync(CancellationToken ct = default)
        {
            if (_repo is null)
                throw new InvalidOperationException("Call Configure(...) before InitializeAsync().");

            WireOnce();

            _reactiveUpdatesEnabled = false;   // <— suspend reactions

            await LoadMonths(ct);

            await LoadMasterLists(ct);
            lueMasterList.ItemIndex = 0;

            await UpdateCurrentListTypeAsync(ct);
            await UpdateCurrentBuyerIdAsync(ct);

            await ReloadOrderMonthsForMasterListAsync(ct);
            lueOrderMonth.ItemIndex = 0;

            SetForecastDate();

            _reactiveUpdatesEnabled = true;    // <— resume reactions
        }

        private async Task UpdateCurrentListTypeAsync(CancellationToken ct)
        {
            if (_masterListRepository is null || MasterListId <= 0) return;
            var ml = await _masterListRepository.GetByIdAsync(MasterListId, ct);
            CurrentListTypeId = ml!.ListTypeId;

            dtForecastDate.Enabled = CurrentListTypeId == ListTypeId.Cosmetics;
            dtForecastFrom.Enabled = dtForecastDate.Enabled;
        }

        private async Task UpdateCurrentBuyerIdAsync(CancellationToken ct)
        {
            if (_masterListRepository is null || MasterListId <= 0) return;
            var ml = await _masterListRepository.GetByIdAsync(MasterListId, ct);
            CurrentBuyerId = ml!.BuyerId;
        }

        private void SetForecastDate()
        {
            var ds = (List<OrderMonthOption>)lueOrderMonth.Properties.DataSource;

            if (ds is null) return;

            var orderMonth = ds.First().OrderMonth;
            dtForecastDate.DateTime = new DateTime(orderMonth.Year, orderMonth.Month, 1).AddMonths(2).AddDays(-1).Date;

            dtForecastFrom.DateTime = DateTime.Today;
        }

        private void WireOnce()
        {
            if (_wired) return;

            // Replace the default GridView with an Advanced Banded Grid View
            _bv = new AdvBandedGridView(gridControl1);
            
            gridControl1.ViewCollection.Add(_bv);
            gridControl1.MainView = _bv;

            // Baseline options (RO/RW handled per-column in schemas)
            GridSchemaShared.ApplyBaselineViewOptions(_bv);

            // Keep your existing non-grid wiring exactly as-is
            seTargetWeeks.Properties.MinValue = 0;
            seTargetWeeks.Properties.MaxValue = 52;
            seTargetWeeks.Properties.Increment = 1;
            seTargetWeeks.EditValue = 8m;

            //barManager1.SetPopupContextMenu(gridControl1, popupMenu1);

            bbiCopyAvailable.ItemClick += (_, __) => CopyProposedToQty_Selected();
            bbiCopyProposed.ItemClick += (_, __) => CopyWeeksToQty_Selected();
            bbiAddExtra.ItemClick += (_, __) => ChangeExtras(true);
            bbiClearExtra.ItemClick += (_, __) => ChangeExtras(false);
            bbiOrderedToConfirmed.ItemClick += (_, __) => CopyOrderedToConfirmed();
            btnSave.Click += async (_, __) => await SaveAsync();

            // If you show a context menu, reference bv instead of the old gv
            popupMenu1.BeforePopup += (_, __) =>
            {
                if (_bv is null) return;
                var selected = _bv.GetSelectedRows().Length;

                // Default disable all, then enable per spec
                bbiCopyAvailable.Enabled = false;
                bbiCopyProposed.Enabled = false;
                bbiAddExtra.Enabled = false;
                bbiClearExtra.Enabled = false;

                if (_menuSpecProvider is null) return;
                foreach (var item in _menuSpecProvider.GetMenuSpec())
                {
                    if (!item.Visible) continue;

                    var enable = _menuSpecProvider.ShouldEnable(item.Action, selected);
                    switch (item.Action)
                    {
                        case ContextAction.CopyProposedToQty:
                            bbiCopyAvailable.Enabled = enable;
                            break;
                        case ContextAction.CopyWeeksToQty:
                            bbiCopyProposed.Enabled = enable;
                            break;
                        case ContextAction.AddExtraCases:
                            bbiAddExtra.Enabled = enable;
                            break;
                        case ContextAction.ClearExtraCases:
                            bbiClearExtra.Enabled = enable;
                            break;
                    }
                }
            };

            // Decide case-by-case where to show YOUR popup vs. built-in
            _bv.PopupMenuShowing += (s, e) =>
            {
                // Let DevExpress show its own header menu
                if (e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.Column)
                    return;

                // (Optional) also let built-in footer/group menus show:
                // if (e.MenuType is GridMenuType.Summary or GridMenuType.Footer or GridMenuType.Group)
                //     return;

                // For data rows/cells/empty space, show YOUR popup
                if (e.HitInfo.InRow || e.HitInfo.InRowCell || e.MenuType == DevExpress.XtraGrid.Views.Grid.GridMenuType.User)
                {
                    e.Allow = false; // suppress DevExpress default in these areas
                    popupMenu1.ShowPopup(Control.MousePosition);
                }
            };

            _wired = true;
        }

        private void BbiOrderedToConfirmed_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ChangeExtras(bool addMode)
        {
            var gv = (GridView)gridControl1.MainView;
            gv.BeginDataUpdate();
            try
            {
                var handles = gv.GetSelectedRows();
                foreach (var h in handles)
                    if (gv.GetRow(h) is VendorOrderRowVM vm)
                        if (addMode)
                            vm.BumpQtyInCases();
                        else
                            vm.ClearExtras();
            }
            finally
            {
                gv.EndDataUpdate();
            }
            gv.RefreshData();
        }


        private void ExportToExcel()
        {
            GridExportHelpers.ExportAdvBandedViewToExcel((AdvBandedGridView)gridControl1.MainView, $"{lueMasterList.Text}-{lueOrderMonth.Text}");
        }

        private async Task LoadMonths(CancellationToken ct)
        {
            if (_repo is null) return;

            // Load month list
            var months = await _repo.ListMonthsAsync(ct);

            lueOrderMonth.Properties.DataSource = months;
        }

        private async Task LoadMasterLists(CancellationToken ct)
        {
            if (_masterListRepository is null) return;

            var lists = await _masterListRepository.GetAllMasterLists(ct);
            lueMasterList.Properties.DataSource = lists;
        }

        // ---------- Data ops ----------

        private async Task LoadGridAsync()
        {
            if (_repo is null || MasterListId == 0 || OrderVendorId == 0 || !ForecastFromDate.HasValue || !ForecastThruDate.HasValue) return;

            await Helpers.OverlayHelper.RunAsync(this, async () =>
            {
                var gv = (GridView)gridControl1.MainView;
                gv.BeginUpdate();
                try
                {
                    _rows.Clear();

                    var data = await _repo.GetGridAsync(MasterListId, OrderVendorId, ForecastFromDate.Value, ForecastThruDate.Value, CancellationToken.None, false);
                    if (data.Count == 0)
                    {
                        var diagResult = XtraMessageBox.Show(this,
                            "There are no items from the Master List added for this month yet.\n\nDo you want to add them now?",
                            "Missing Data",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        switch (diagResult)
                        {
                            case DialogResult.Yes:
                                data = await _repo.GetGridAsync(MasterListId, OrderVendorId, ForecastFromDate.Value, ForecastThruDate.Value, CancellationToken.None, true);
                                break;

                            case DialogResult.No:
                                break;

                            default:
                                break;
                        }
                    }

                    bool useConfirmedValues = data.Any(d => d.QtyConfirmed.HasValue);
                    foreach (var r in data)
                    {
                        var vm = new VendorOrderRowVM(r)
                        {
                            UseWeeksForPallets = CurrentListTypeId == ListTypeId.Haircare,
                            UseConfirmed = useConfirmedValues,
                            IsDirty = false
                        };
                        _rows.Add(vm);
                    }

                    var schema = _schemaFactory.Resolve(CurrentListTypeId);
                    schema.Build(_bv!, gridControl1, _rows, CurrentBuyerId);

                    _gridPersistence.RestoreSettings(gridControl1, Convert.ToString((int)CurrentListTypeId));

                    BandDividerHelper.Enable(_bv!, thickness: 2, drawHeader: true, lineColor: Color.DarkGray);

                    // configure context menu for this ListType
                    ApplyContextMenuForListType(CurrentListTypeId);

                    btnSave.Enabled = data.Count > 0;
                }
                finally
                {
                    gv.BestFitColumns();
                    gv.EndUpdate();

                    RefreshViewHard(_bv!);
                }
            });
        }

        private void ApplyContextMenuForListType(ListTypeId listTypeId)
        {
            _menuSpecProvider = _menuSpecFactory.Resolve(listTypeId);
            var spec = _menuSpecProvider.GetMenuSpec();

            // Map spec → your private BarItems (keep BarItems private)
            foreach (var item in spec)
            {
                switch (item.Action)
                {
                    case ContextAction.CopyProposedToQty:
                        bbiCopyAvailable.Visibility = item.Visible ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
                        break;

                    case ContextAction.CopyWeeksToQty:
                        bbiCopyProposed.Visibility = item.Visible ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
                        break;

                    case ContextAction.AddExtraCases:
                        bbiAddExtra.Visibility = item.Visible ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
                        break;

                    case ContextAction.ClearExtraCases:
                        bbiClearExtra.Visibility = item.Visible ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
                        break;

                    case ContextAction.CopyOrderedToConfirmed:
                        bbiClearExtra.Visibility = item.Visible ? DevExpress.XtraBars.BarItemVisibility.Always : DevExpress.XtraBars.BarItemVisibility.Never;
                        break;
                }
            }
        }

        private void CopyProposedToQty_Selected()
        {
            var gv = (GridView)gridControl1.MainView;
            gv.BeginDataUpdate();
            try
            {
                var handles = gv.GetSelectedRows();
                foreach (var h in handles)
                    if (gv.GetRow(h) is VendorOrderRowVM vm)
                        vm.QtyInCases = vm.AvailableQtyInCases + vm.ExtraCases;
            }
            finally
            {
                gv.EndDataUpdate();
            }
            gv.RefreshData();
        }

        private void CopyOrderedToConfirmed()
        {
            var gv = (GridView)gridControl1.MainView;
            gv.BeginDataUpdate();
            try
            {
                var handles = gv.GetSelectedRows();
                foreach (var h in handles)
                    if (gv.GetRow(h) is VendorOrderRowVM vm)
                        vm.QtyConfirmed = vm.QtyInCases;
            }
            finally
            {
                gv.EndDataUpdate();
            }
            gv.RefreshData();
        }

        private void CopyWeeksToQty_Selected()
        {
            var bv = (AdvBandedGridView)gridControl1.MainView;
            bv.BeginDataUpdate();
            try
            {
                foreach (var h in bv.GetSelectedRows())
                {
                    if (bv.GetRow(h) is VendorOrderRowVM vm)
                    {
                        var runRate = Math.Max(0m, vm.WeeklyRunRate);
                        var weeks = Math.Max(0, vm.WeeksToBuy ?? 0);
                        var qtyCases = (int)Math.Ceiling(runRate * weeks);
                        vm.QtyInCases = qtyCases;
                    }
                }
            }
            finally
            {
                bv.EndDataUpdate();
                bv.RefreshData();
            }
        }

        private async Task SaveAsync()
        {
            _gridPersistence.SaveSettings(gridControl1, Convert.ToString((int)CurrentListTypeId));

            if (_repo is null || OrderVendorId <= 0) return;

            // Ensure the active editor commits its value to the data source
            var view = (AdvBandedGridView)gridControl1.MainView;
            view.PostEditor();
            view.UpdateCurrentRow();

            // Collect only dirty rows
            var dirtyRows = _rows.OfType<VendorOrderRowVM>()
                                 .Where(r => r.IsDirty)
                                 .ToList();

            if (dirtyRows.Count == 0)
            {
                XtraMessageBox.Show(this, "No changes to save.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Map to your DTO
            var edits = dirtyRows.Select(r => new OrderQtyEdit
            {
                UpcId = r.UpcId,
                QtyInCases = r.QtyInCases,
                ExtraCases = r.ExtraCases,
                QtyConfirmed = r.QtyConfirmed ?? 0,
                WeeksToBuy = r.WeeksToBuy ?? 0,
            }).ToList();

            try
            {
                // Optionally disable Save UI here
                await _repo.UpdateQuantitiesAsync(OrderVendorId, edits, CancellationToken.None);

                // Clear IsDirty only for rows we actually saved
                foreach (var row in dirtyRows)
                    row.IsDirty = false;

                XtraMessageBox.Show(this, "Quantities saved.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(this, $"Save failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void lueOrderMonth_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
            {
                if (_repo is null) return;

                var newId = await _repo.AddNewOrderMonthAsync(MasterListId, OrderVendorId);
                await LoadMonths(CancellationToken.None);

                lueOrderMonth.ItemIndex = 0;
                SetForecastDate();
            }
        }

        private async void btnRefreshData_Click(object sender, EventArgs e)
        {
            // Ensure required values
            if (MasterListId <= 0 || OrderVendorId <= 0 || !ForecastFromDate.HasValue || !ForecastThruDate.HasValue)
            {
                XtraMessageBox.Show(this,
                    "Master List, Order Month, Forecast From, and Forecast To are required.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            await PromptToSaveIfDirtyAsync(this);
            await LoadGridAsync();
        }

        public async Task<bool> PromptToSaveIfDirtyAsync(IWin32Window owner)
        {
            if (!IsDirty) return true;

            var res = XtraMessageBox.Show(owner, "You have unsaved changes. Save changes?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (res == DialogResult.Cancel) return false;
            if (res == DialogResult.Yes)
            {
                try
                {
                    await SaveAsync(); // your existing SaveAsync (only dirty rows)
                    return true;
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(owner, $"Save failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            // No = discard
            // Clear flags explicitly so subsequent checks don’t re-prompt
            foreach (var r in _rows) r.IsDirty = false;

            return true;
        }

        private bool StopForAlternateBuyerWithQuantity()
        {
            if (_rows.Any(w => w.HasAlternateBuyer && w.QtyInCases > 0))
                if (XtraMessageBox.Show($"You currently have quantities for items items that should be bought at alternate buyers.\n\nAre you sure you want to continue?", "Alternate Buyer Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No) 
                    return true;
            
            return false;
        }

        private bool StopForNoConfirmedQuantities()
        {
            if (!_rows.Any(w => w.QtyConfirmed > 0))
            { 
                XtraMessageBox.Show($"There are no confirmed quantities to generate a PO for.", "Invalid Quantities Specified", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return true;
            }

            return false;
        }

        private async void btnMainframeCsv_Click(object sender, EventArgs e)
        {
            var view = gridControl1.MainView as AdvBandedGridView;
            view?.CloseEditor();
            view?.UpdateCurrentRow();

            if (!await PromptToSaveIfDirtyAsync(this))
                return;

            if (StopForNoConfirmedQuantities())
                return;

            if (StopForAlternateBuyerWithQuantity())
                return;

            // 2) Prompt user for save location
            using var dlg = new SaveFileDialog
            {
                Title = "Save CSV",
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                DefaultExt = "csv",
                FileName = $"Mainframe PO File - {lueMasterList.Text} ({lueOrderMonth.Text}).csv",
                RestoreDirectory = true
            };

            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            try
            {
                // 3) Build CSV from the ViewModel
                //    Adjust to how you access your VM/rows:
                //    e.g., var rows = _viewModel.Rows;
                string csv = VendorOrderRowVM.BuildMainframeCsv(_rows);

                // 4) Write with UTF-8 BOM (Excel-friendly)
                File.WriteAllText(dlg.FileName, csv, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

                var dr = XtraMessageBox.Show(
                    this,
                    $"CSV exported successfully.\n\nOpen containing folder now?",
                    "Success",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                if (dr == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{dlg.FileName}\"");
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(this, $"Failed to export CSV:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }

        private async void btnPopulateVendorOrder_Click(object? sender, EventArgs e)
        {
            var view = gridControl1.MainView as AdvBandedGridView;
            view?.PostEditor();
            view?.UpdateCurrentRow();

            if (!await PromptToSaveIfDirtyAsync(this))
                return;

            if (StopForAlternateBuyerWithQuantity())
                return;

            // Prompt for vendor workbook here (outside the overlay)
            string? path = null;
            using (var dlg = new OpenFileDialog
            {
                Title = "Select Vendor Order Sheet (.xlsx)",
                Filter = "Excel Workbook (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                RestoreDirectory = true
            })
            {
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                path = dlg.FileName;
            }

            if (!string.Equals(Path.GetExtension(path), ".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                XtraMessageBox.Show(this, "Only .xlsx files are supported.", "Unsupported File",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            VendorOrderResult? result = null;

            await Helpers.OverlayHelper.RunAsync(this, async () =>
            {
                result = await OrderPlacementHelper.PopulateVendorOrderSheetAsync(
                    _rows,
                    path!,
                    CurrentListTypeId.ToString(),
                    lueOrderMonth.Text
                );
            });

            if (result is null)
                return;

            if (result.Updated == 0)
            {
                XtraMessageBox.Show(this, "No rows had Qty to Order > 0.", "Nothing to Do",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var dr = XtraMessageBox.Show(
                    this,
                    $"Updated {result.Updated:n0} row(s).\n\nOpen containing folder now?",
                    "Success",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                if (dr == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{result.SavedPath}\"");
                }
            }
        }

        // Central reaction to MasterList changes
        private async Task OnMasterListChangedAsync(int oldValue, int newValue)
        {
            // 1) protect user edits
            if (!await PromptToSaveIfDirtyAsync(this))
            {
                // user canceled -> revert selection and exit
                _reactiveUpdatesEnabled = false;
                try
                {
                    _masterListId = oldValue;
                    lueMasterList.EditValue = oldValue;
                }
                finally
                {
                    _reactiveUpdatesEnabled = true;
                }
                return;
            }

            // 2) refresh "Order For" (OrderVendor) options for this Master List
            await ReloadOrderMonthsForMasterListAsync(CancellationToken.None);

            // 2b) cache the ListType for this MasterList
            await UpdateCurrentListTypeAsync(CancellationToken.None);
            await UpdateCurrentBuyerIdAsync(CancellationToken.None);

            // 3) clear dependent picks + grid until user chooses a new order
            _reactiveUpdatesEnabled = false;
            try
            {
                lueOrderMonth.EditValue = null; // forces user to pick from the filtered list
                OrderVendorId = 0;
            }
            finally
            {
                _reactiveUpdatesEnabled = true;
            }

            _rows.Clear();
            btnSave.Enabled = false;

            // Optionally update your forecast dates if you want a default range
            SetForecastDate();

            // Gently prompt the user to pick an order month after changing Master List
            if (lueOrderMonth.IsHandleCreated && lueOrderMonth.Visible && lueOrderMonth.Enabled)
            {
                // Use BeginInvoke so the popup happens after EditValue/DataSource updates settle
                lueOrderMonth.BeginInvoke(new Action(() => lueOrderMonth.ShowPopup()));
            }
        }

        // Filter your existing months list (swap to repo-side filtering when ready)
        private async Task ReloadOrderMonthsForMasterListAsync(CancellationToken ct)
        {
            if (_repo is null) return;

            var all = await _repo.ListMonthsAsync(ct);
            var mlId = MasterListId;
            var filtered = (mlId > 0) ? all.Where(m => m.MasterListId == mlId).ToList() : all;

            lueOrderMonth.Properties.BeginUpdate();
            try
            {
                lueOrderMonth.Properties.DataSource = filtered;
            }
            finally
            {
                lueOrderMonth.Properties.EndUpdate();
            }
        }

        private static void RefreshViewHard(AdvBandedGridView bv)
        {
            if (bv is null) return;

            var gc = bv.GridControl;

            // Commit any in-place editor value
            bv.CloseEditor();
            bv.PostEditor();
            bv.UpdateCurrentRow();

            // Make sure control is fully initialized
            gc.ForceInitialize();

            // Recompute layout + visuals in a safe batch
            bv.BeginUpdate();
            try
            {
                bv.LayoutChanged();   // re-run columns/bands layout
            }
            finally
            {
                bv.EndUpdate();
            }

            // Rebind/redraw data & UI
            bv.RefreshData();
            bv.BestFitColumns();      // optional, if you rely on best-fit
            gc.Refresh();             // forces a control repaint
            bv.Invalidate();          // belt-and-suspenders: repaint the view surface
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            var host = FindForm();
            if (host != null)
                host.FormClosing += Host_FormClosing;
        }

        private void Host_FormClosing(object? sender, FormClosingEventArgs e)
        {
            try
            {
                if (_bv != null && (int)CurrentListTypeId > 0)
                    _gridPersistence.SaveSettings(gridControl1, Convert.ToString((int)CurrentListTypeId));
            }
            catch { /* ignore */ }
        }
    }
}
