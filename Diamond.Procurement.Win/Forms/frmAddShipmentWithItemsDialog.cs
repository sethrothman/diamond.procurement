using DevExpress.XtraEditors;
using Diamond.Procurement.App.Processing;
using Diamond.Procurement.Data.Repositories;
using Diamond.Procurement.Win.Helpers;
using System.ComponentModel;
using System.Data;
using System.IO;

namespace Diamond.Procurement.Win.Forms
{
    public partial class frmAddShipmentWithItemsDialog : DevExpress.XtraEditors.XtraForm
    {
        private readonly IShipmentItemsProcessor _shipmentProcessor = new LibertyShipmentItemsProcessor();
        private bool _gridWired;

        private readonly IOrderVendorShipmentRepository _repo;
        private readonly int _orderVendorId;
        private BindingList<ItemRow> _rows = new();

        public frmAddShipmentWithItemsDialog(IOrderVendorShipmentRepository repo, int orderVendorId)
        {
            _repo = repo;
            _orderVendorId = orderVendorId;

            InitializeComponent();

            // Enable form-level key handling
            this.KeyPreview = true;
            this.KeyDown += OnDialogKeyDown;

            //btnPaste.Click += (_, __) => PasteFromClipboard();
            //btnImportCsv.Click += async (_, __) => await ImportFromFileAsync();
            //btnClear.Click += (_, __) => _rows.Clear();

            gridItems.AllowDrop = true;
            gridItems.DragEnter += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    var p = ((string[])e.Data.GetData(DataFormats.FileDrop))?.FirstOrDefault();
                    if (!string.IsNullOrEmpty(p) && (p.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) || p.EndsWith(".xls", StringComparison.OrdinalIgnoreCase)))
                    { e.Effect = DragDropEffects.Copy; return; }
                }
                e.Effect = DragDropEffects.None;
            };
            gridItems.DragDrop += async (s, e) =>
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files == null || files.Length == 0) return;
                using var _ = BusyScope.Show(this);
                await LoadShipmentFileAsync(files[0]);
            };
            gridItems.DataSource = _rows;

            WireOnce();

            // Defaults
            dateShipment.DateTime = DateTime.Today;
            dateEta.DateTime = DateTime.Today.AddDays(7);
        }

        public sealed class ItemRow
        {
            public string? Upc { get; set; }
            public int Quantity { get; set; }
            public int? UpcId { get; set; }
            public string? Status { get; set; }
        }


        private void WireOnce()
        {
            if (_gridWired) return;
            _gridWired = true;

            var v = gridView1;
            v.BeginUpdate();
            try
            {
                v.Appearance.HeaderPanel.FontStyleDelta = FontStyle.Bold;

                v.Columns.Clear();

                // UPC (string)
                var colUpc = v.Columns.AddVisible(nameof(ItemRow.Upc), "UPC");
                colUpc.OptionsColumn.AllowEdit = true;
                colUpc.OptionsColumn.AllowFocus = true;
                colUpc.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                colUpc.Width = 180;

                // Quantity (int)
                var colQty = v.Columns.AddVisible(nameof(ItemRow.Quantity), "Quantity");
                colQty.OptionsColumn.AllowEdit = true;
                colQty.OptionsColumn.AllowFocus = true;
                colQty.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                colQty.DisplayFormat.FormatString = "n0";
                colQty.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                colQty.SummaryItem.DisplayFormat = "{0:n0}";
                colQty.Width = 110;

                // UpcId (hidden)
                var colUpcId = v.Columns.AddField(nameof(ItemRow.UpcId));
                colUpcId.Visible = false;

                // Status (readonly string)
                var colStatus = v.Columns.AddField(nameof(ItemRow.UpcId));
                colStatus.Visible = false;

                //var colStatus = v.Columns.AddVisible(nameof(ItemRow.Status), "Status");
                //colStatus.OptionsColumn.AllowEdit = false;
                //colStatus.OptionsColumn.AllowFocus = false;
                //colStatus.Width = 180;

                // Editors
                var riUpc = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit
                {
                    CharacterCasing = CharacterCasing.Upper
                };
                // allow digits and a few common chars; adjust if you want only digits
                riUpc.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
                riUpc.Mask.EditMask = @"[0-9A-Z\- ]+";
                riUpc.Mask.UseMaskAsDisplayFormat = false;

                var riQty = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit
                {
                    IsFloatValue = false,
                    MinValue = 0,
                    MaxValue = int.MaxValue,
                    Increment = 1
                };
                riQty.EditMask = "n0";

                gridItems.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] { riUpc, riQty });
                colUpc.ColumnEdit = riUpc;
                colQty.ColumnEdit = riQty;

                // View options
                v.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
                v.OptionsView.ShowFooter = true;
                v.OptionsView.ShowGroupPanel = false;
                v.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.Click;
                v.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
                v.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.True;
                v.OptionsSelection.MultiSelect = true;
                v.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;

                // Row validation & inline feedback
                v.ValidateRow += (s, e) =>
                {
                    var row = (ItemRow)v.GetRow(e.RowHandle);
                    if (row == null) return;

                    string msg = null;
                    if (string.IsNullOrWhiteSpace(row.Upc))
                        msg = "UPC is required.";
                    else if (row.Quantity <= 0)
                        msg = "Quantity must be > 0.";
                    else if (row.UpcId == null && !string.IsNullOrWhiteSpace(row.Upc))
                        msg = "Unknown UPC. Use Paste or Resolve to map.";

                    if (msg != null)
                    {
                        e.Valid = false;
                        v.SetColumnError(colUpc, msg);
                    }
                    else
                    {
                        e.Valid = true;
                        v.ClearColumnErrors();
                    }
                };

                // Immediate cell-level guard for Quantity
                v.ValidatingEditor += (s, e) =>
                {
                    if (v.FocusedColumn == colQty)
                    {
                        if (!int.TryParse(Convert.ToString(e.Value), out var q) || q < 0)
                        {
                            e.Valid = false;
                            e.ErrorText = "Enter a non-negative whole number.";
                        }
                    }
                };

                // Colorize “Status” issues
                v.RowStyle += (s, e) =>
                {
                    var row = (ItemRow)v.GetRow(e.RowHandle);
                    if (row == null) return;
                    if (!string.IsNullOrEmpty(row.Status))
                    {
                        e.Appearance.ForeColor = System.Drawing.Color.Firebrick;
                    }
                };

                // Convenience keys
                v.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Delete && !v.IsEditing)
                    {
                        // Delete selected rows
                        foreach (var handle in v.GetSelectedRows().OrderByDescending(h => h))
                            v.DeleteRow(handle);
                        e.Handled = true;
                    }
                };

                // Friendly widths
                v.BestFitColumns();
                colUpc.Width = Math.Max(colUpc.Width, 150);
                colQty.Width = 100;
                colStatus.Width = Math.Max(colStatus.Width, 160);
            }
            finally
            {
                v.EndUpdate();
            }
        }

        private void PasteFromClipboard()
        {
            var text = Clipboard.GetText(TextDataFormat.Text);
            if (string.IsNullOrWhiteSpace(text)) return;

            // Excel pastes as tab-delimited rows; also support CSV
            var lines = text.Replace("\r\n", "\n").Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var cells = line.Split('\t');
                if (cells.Length < 2) cells = line.Split(','); // fallback CSV

                if (cells.Length >= 2)
                {
                    var rawUpc = cells[0]?.Trim();
                    var qtyStr = cells[1]?.Trim();
                    if (string.IsNullOrWhiteSpace(rawUpc)) continue;

                    int qty = 0;
                    int.TryParse(qtyStr, out qty);
                    _rows.Add(new ItemRow { Upc = rawUpc, Quantity = qty });
                }
            }
        }

        private async Task LoadShipmentFileAsync(string path)
        {
            // Optional: if you add a dispatcher later, use that here.
            if (!_shipmentProcessor.CanHandle(path))
            {
                XtraMessageBox.Show(this, "There is no processor to handle this Excel file.", "Import", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = await Task.Run(() => _shipmentProcessor.Parse(path));

            if (!string.IsNullOrWhiteSpace(result.InvoiceNum))
                textEdit1.Text = result.InvoiceNum!;
            if (result.TruckNum.HasValue)
                spinTruckNum.Value = result.TruckNum.Value;

            _rows.Clear();
            foreach (var it in result.Items)
                _rows.Add(new ItemRow { Upc = it.Upc, Quantity = it.Quantity });

            gridItems.RefreshDataSource();
        }

        private async Task ResolveUpcsAsync()
        {
            var inputs = _rows.Where(r => !string.IsNullOrWhiteSpace(r.Upc))
                              .Select(r => r.Upc!.Trim())
                              .ToList();
            if (inputs.Count == 0) return;

            var resolutions = await _repo.ResolveUpcsAsync(inputs);

            // Map back by original input (case-insensitive)
            var byInput = resolutions
                .GroupBy(r => r.Input, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            foreach (var row in _rows)
            {
                if (string.IsNullOrWhiteSpace(row.Upc)) continue;
                if (byInput.TryGetValue(row.Upc.Trim(), out var res))
                {
                    row.UpcId = res.UpcId;
                    row.Status = res.UpcId == null
                        ? "Unknown UPC"
                        : (row.Quantity > 0 ? null : "Qty<=0");
                }
                else
                {
                    row.UpcId = null;
                    row.Status = "Unknown UPC";
                }
            }
            gridItems.RefreshDataSource();
        }

        private bool ValidateRows(out string error)
        {
            error = "";
            if (_rows.Count == 0) { error = "No rows."; return false; }

            var bad = _rows.Where(r => r.UpcId == null || r.Quantity <= 0).ToList();
            if (bad.Count > 0)
            {
                error = "Some rows have Unknown UPC or non-positive Quantity.";
                return false;
            }
            return true;
        }

        private async Task<bool> SaveAsync()
        {
            await ResolveUpcsAsync();
            if (!ValidateRows(out var err))
            {
                XtraMessageBox.Show(this, err, "Cannot Save", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var details = _rows
                .Where(r => r.UpcId != null && r.Quantity > 0)
                .Select(r => (UpcId: r.UpcId!.Value, Quantity: r.Quantity))
                .ToList();

            var newId = await _repo.InsertShipmentWithDetailsAsync(
                orderVendorId: _orderVendorId,
                truckNum: (int)spinTruckNum.Value,
                InvoiceNum: textEdit1.Text,
                shipmentDate: dateShipment.DateTime.Date,
                estimatedDeliveryDate: dateEta.DateTime.Date,
                details: details);

            this.Tag = newId; // so caller can focus it
            return true;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            using var _ = BusyScope.Show(this);
            if (await SaveAsync())
                DialogResult = DialogResult.OK;
        }

        private void OnDialogKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                PasteFromClipboard();
                e.Handled = true;   // mark as handled
            }
        }
    }
}