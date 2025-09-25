using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using Diamond.Procurement.Data;
using Diamond.Procurement.Data.Repositories;
using Diamond.Procurement.Domain.Models;
using Diamond.Procurement.Win.Forms;
using Diamond.Procurement.Win.Helpers;
using Diamond.Procurement.Win.ViewModels;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;

namespace Diamond.Procurement.Win.UserControls
{
    public partial class OrderVendorShipmentPage : XtraUserControl
    {
        private int _orderVendorId;
        private bool _lueConfigured;

        private MasterListRepository? _masterListRepository;
        private readonly IOrderVendorShipmentRepository _shipmentRepo;
        private readonly IOrderVendorRepository _repo;

        // Matrix datasource
        private DataTable? _matrix;
        private readonly BindingSource _matrixBs = new();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int OrderVendorId
        {
            get => _orderVendorId;
            set
            {
                if (_orderVendorId == value) return;
                _orderVendorId = value;
                _ = RebuildMatrixAsync();  // auto-refresh when month/vendor changes
            }
        }

        public OrderVendorShipmentPage(IOrderVendorShipmentRepository shipmentRepo, IOrderVendorRepository repo, MasterListRepository? masterListRepository)
        {
            InitializeComponent();

            _masterListRepository = masterListRepository;
            _shipmentRepo = shipmentRepo;
            _repo = repo;

            // Bind LookUpEdit to OrderVendorId (as before)
            lueOrderMonth.DataBindings.Add(
                "EditValue", this, nameof(OrderVendorId),
                true, DataSourceUpdateMode.OnPropertyChanged, 0);

            // Grid
            gridMatrix.DataSource = _matrixBs;
            WireOnce();

            // Buttons
            btnAddShipment.Click += async (_, __) => await AddShipmentAsync();
            btnRefresh.Click += async (_, __) => await RebuildMatrixAsync();
        }

        public async Task InitializeAsync()
        {
            await LoadMonths(CancellationToken.None);
            //lueOrderMonth.ItemIndex = 0; // triggers Rebuild via databinding
        }

        private async Task LoadMonths(CancellationToken ct)
        {
            var months = await _repo.ListMonthsAsync(ct);
            lueOrderMonth.Properties.DataSource = months;

            // Configure after binding (first time only)
            if (!_lueConfigured)
            {
                ConfigureOrderMonthLookup();
                _lueConfigured = true;
            }
        }

        private void ConfigureOrderMonthLookup()
        {
            var props = lueOrderMonth.Properties;

            // Display/Value member
            props.DisplayMember = nameof(OrderMonthOption.DisplayForLookupEdit);
            props.ValueMember = nameof(OrderMonthOption.OrderVendorId);


            // Search behavior
            props.PopupFilterMode = PopupFilterMode.Contains; // type-to-filter anywhere
            props.NullText = "(Select Desired Order)";
            props.NullValuePrompt = props.NullText;

            // The popup view is a GridView (SearchLookUpEditView)
            var view = (GridView)props.View;
            view.BeginUpdate();
            try
            {
                view.Columns.Clear();

                // Visible columns in the dropdown (order matters)
                var colMasterList = view.Columns.AddVisible(nameof(OrderMonthOption.MasterList), "Master List");
                var colFullMonthYear = view.Columns.AddVisible(nameof(OrderMonthOption.FullMonthYear), "Month");
                var colVendor = view.Columns.AddVisible(nameof(OrderMonthOption.Vendor), "Vendor");

                // Group by Master List
                colMasterList.GroupIndex = 0;

                // If you want to HIDE the grouped column while keeping grouping, set:
                view.OptionsView.ShowGroupedColumns = false;   // keeps groups, hides the column

                // General view niceties
                view.OptionsView.ShowGroupPanel = false;
                view.OptionsBehavior.AutoExpandAllGroups = true; // expand by default
                view.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
                view.OptionsSelection.EnableAppearanceFocusedCell = false;

                // Optional: sort inside each group
                colFullMonthYear.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
                colFullMonthYear.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending;

                // Size to content
                view.BestFitColumns();
            }
            finally
            {
                view.EndUpdate();
            }
        }

        private async Task RebuildMatrixAsync()
        {
            if (OrderVendorId <= 0) return;

            using var _ = BusyScope.Show(this);

            // 1) Pull shipments for selected OrderVendorId
            var shipments = await _shipmentRepo.GetShipmentsAsync(OrderVendorId); // date + id + trucknum

            // 2) Base rows (all ordered items) — UpcId, UPC, Description, QtyInCases
            //    Add this method to your OrderVendor repo if you don’t already have it.
            //    SQL sample is shown below.
            var baseRows = await _repo.ListOrderVendorDetailForMatrixAsync(OrderVendorId, CancellationToken.None); // returns List<MatrixBaseRow>

            // 3) Build DataTable with dynamic cols (one per shipment)
            _matrix = new DataTable("ShipmentMatrix");
            _matrix.Columns.Add("UpcId", typeof(int));
            _matrix.Columns.Add("UPC", typeof(string));
            _matrix.Columns.Add("Description", typeof(string));
            _matrix.Columns.Add("Price", typeof(decimal));
            _matrix.Columns.Add("QtyInCases", typeof(int));

            // after you create _matrix and add base columns UpcId/UPC/Description/QtyInCases
            var colNameByShipmentId = new Dictionary<int, string>();

            string AddShipmentColumn(int shipmentId, DateTime? shipDate, int? truckNum, string? invoiceNum)
            {
                // human-friendly caption if we have metadata; otherwise fallback
                var baseName = shipDate.HasValue
                    ? $"{shipDate:yyyy-MM-dd}\n{invoiceNum} (T{truckNum})"
                    : $"Shipment {shipmentId}";

                // ensure uniqueness
                var name = baseName;
                var n = 1;
                while (_matrix.Columns.Contains(name))
                    name = $"{baseName} {++n}";

                // actually add column to the DataTable
                var dc = _matrix.Columns.Add(name, typeof(int));
                dc.DefaultValue = 0;

                // IMPORTANT: columns added after rows -> initialize existing rows
                foreach (DataRow rr in _matrix.Rows)
                    rr[name] = 0;

                colNameByShipmentId[shipmentId] = name;
                return name;
            }

            foreach (var s in shipments.OrderBy(s => s.ShipmentDate).ThenBy(s => s.OrderVendorShipmentId))
            {
                AddShipmentColumn(s.OrderVendorShipmentId, s.ShipmentDate, s.TruckNum, s.InvoiceNum);
            }

            // 4) Seed matrix rows with base data
            var rowIndexByUpcId = new Dictionary<int, DataRow>();
            foreach (var b in baseRows.OrderBy(b => b.UPC))
            {
                var r = _matrix.NewRow();
                r["UpcId"] = b.UpcId;
                r["UPC"] = b.UPC;
                r["Description"] = b.Description ?? "";
                r["Price"] = b.Price;
                r["QtyInCases"] = b.QtyInCases;
                // default 0 for all shipment columns
                foreach (DataColumn c in _matrix.Columns)
                    if (c.DataType == typeof(int) && c.ColumnName != "UpcId" && c.ColumnName != "QtyInCases")
                        r[c.ColumnName] = 0;

                _matrix.Rows.Add(r);
                rowIndexByUpcId[b.UpcId] = r;
            }

            // Get all aggregated quantities once
            var all = await _shipmentRepo.GetShipmentQuantitiesAsync(OrderVendorId);
            // -> rows: OrderVendorShipmentId, UpcId, Quantity

            if (all.Count > 0)
            {
                foreach (var grp in all.GroupBy(x => x.OrderVendorShipmentId))
                {
                    var shipmentId = grp.Key;

                    // Ensure a column exists for this shipment
                    if (!colNameByShipmentId.TryGetValue(shipmentId, out var colName))
                    {
                        // Try to name it nicely if we have metadata
                        var meta = shipments.FirstOrDefault(s => s.OrderVendorShipmentId == shipmentId);
                        colName = AddShipmentColumn(shipmentId, meta?.ShipmentDate, meta?.TruckNum, meta?.InvoiceNum);
                    }

                    // Final guard (should never trip, but keeps things safe)
                    if (!_matrix.Columns.Contains(colName))
                        colName = AddShipmentColumn(shipmentId, null, null, null);

                    // Write quantities
                    foreach (var row in grp)
                    {
                        if (rowIndexByUpcId.TryGetValue(row.UpcId, out var dr))
                            dr[colName] = row.Quantity;
                        // else: shipped UPC not in order base; decide if you want to append a new row
                    }
                }
            }

            AddReceivedSumColumn();
            AddRemainingColumn();

            _matrix.AcceptChanges();

            // Rebind + force columns to regenerate
            _matrixBs.DataSource = _matrix;
            gridMatrix.DataSource = _matrixBs;

            viewMatrix.Columns.Clear();
            gridMatrix.ForceInitialize();
            viewMatrix.PopulateColumns();

            BuildBandsAndStyle((AdvBandedGridView)viewMatrix);
        }

        private void AddRemainingColumn()
        {
            if (_matrix == null) return;

            // If we’re rebuilding, drop & recreate
            if (_matrix.Columns.Contains("RemainingCases"))
                _matrix.Columns.Remove("RemainingCases");

            var expr = BuildRemainingExpression(_matrix);
            // If you want to clamp negatives to 0, use:
            // expr = $"IIF(({expr}) < 0, 0, ({expr}))";

            var col = new DataColumn("RemainingCases", typeof(int))
            {
                Expression = expr
            };
            _matrix.Columns.Add(col);
        }

        private void AddReceivedSumColumn()
        {
            if (_matrix == null) return;

            const string name = "TotalReceivedCases";
            if (_matrix.Columns.Contains(name))
                _matrix.Columns.Remove(name);

            var expr = BuildShipmentsSumExpression(_matrix);
            var col = new DataColumn(name, typeof(int));

            if (expr == "0")
                col.DefaultValue = 0;     // no shipment columns yet
            else
                col.Expression = expr;     // auto-updating sum

            _matrix.Columns.Add(col);
        }

        private static string BuildShipmentsSumExpression(DataTable dt)
        {
            var parts = new List<string>();
            foreach (DataColumn c in dt.Columns)
            {
                if (c.DataType == typeof(int) && !BaseFields.Contains(c.ColumnName))
                    parts.Add(Wrap(dt, c.ColumnName));
            }
            // No shipments? return "0" so Expression can be skipped
            return parts.Count == 0 ? "0" : string.Join(" + ", parts);
        }

        private static string BuildRemainingExpression(DataTable dt)
        {
            // QtyInCases - (sum of all shipment columns)
            var parts = new List<string> { Wrap(dt, "QtyInCases") };

            foreach (DataColumn c in dt.Columns)
            {
                if (c.DataType == typeof(int) && !BaseFields.Contains(c.ColumnName))
                {
                    parts.Add($"- {Wrap(dt, c.ColumnName)}");
                }
            }

            return string.Join(" ", parts);
        }

        private static string Wrap(DataTable dt, string columnName)
        {
            // Safely wrap column identifiers (handles spaces, parentheses, even newlines)
            return "[" + columnName.Replace("]", "]]") + "]";
        }

        private static readonly HashSet<string> BaseFields = new(StringComparer.OrdinalIgnoreCase)
        {
            "UpcId", "UPC", "Description", "Price", "QtyInCases", "RemainingCases", "TotalReceivedCases"
        };

        private void BuildBandsAndStyle(AdvBandedGridView v)
        {
            v.BeginUpdate();
            try
            {
                v.Bands.Clear();

                var itemBand = v.Bands.AddBand("Item");
                itemBand.AppearanceHeader.FontStyleDelta = FontStyle.Bold;
                itemBand.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;

                var shipmentsBand = v.Bands.AddBand("Shipments");
                shipmentsBand.AppearanceHeader.FontStyleDelta = FontStyle.Bold;
                shipmentsBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

                // Global header wrapping
                v.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True;
                v.Appearance.HeaderPanel.Options.UseTextOptions = true;
                v.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;

                BandedGridColumn? Col(string name) => v.Columns[name] as BandedGridColumn;

                // -------- Base / Item band --------
                var upcId = Col("UpcId");
                if (upcId != null) { upcId.Visible = false; upcId.OwnerBand = itemBand; }

                var upc = Col("UPC");
                if (upc != null) 
                {
                    upc.Caption = "UPC"; upc.Width = 140; upc.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left; upc.OwnerBand = itemBand;
                    upc.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Count;
                    upc.SummaryItem.DisplayFormat = "{0:n0} Items";
                }

                var desc = Col("Description");
                if (desc != null) { desc.Caption = "Description"; desc.Width = 280; desc.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left; desc.OwnerBand = itemBand; }

                var price = Col("Price");
                if (price != null)
                {
                    price.Caption = "Price"; price.Width = 140;
                    price.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    price.DisplayFormat.FormatString = "c2";
                    price.OwnerBand = itemBand;
                }

                var qty = Col("QtyInCases");
                if (qty != null)
                {
                    qty.Caption = "Cases Ordered";
                    qty.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    qty.DisplayFormat.FormatString = "n0";
                    qty.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    qty.SummaryItem.DisplayFormat = "{0:n0}";
                    qty.Width = 110;
                    qty.OwnerBand = itemBand;
                }

                var rem = Col("RemainingCases");
                if (rem != null)
                {
                    rem.Caption = "Cases Remaining";
                    rem.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    rem.DisplayFormat.FormatString = "n0";
                    rem.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    rem.SummaryItem.DisplayFormat = "{0:n0}";
                    rem.Width = 110;
                    rem.OwnerBand = itemBand;
                }

                // Hide helper column BEFORE assigning shipments to avoid it being grabbed
                var total = Col("TotalReceivedCases");
                if (total != null)
                {
                    total.OwnerBand = itemBand;
                    total.Visible = false;
                    total.VisibleIndex = -1;
                    total.OptionsColumn.ShowInCustomizationForm = false;
                }

                // -------- Shipments band (anything not in BaseFields) --------
                foreach (BandedGridColumn c in v.Columns)
                {
                    if (BaseFields.Contains(c.FieldName)) continue; // already handled

                    c.OwnerBand = shipmentsBand;
                    c.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                    c.DisplayFormat.FormatString = "n0";
                    c.SummaryItem.SummaryType = DevExpress.Data.SummaryItemType.Sum;
                    c.SummaryItem.DisplayFormat = "{0:n0}";
                    c.MinWidth = 90;

                    // header wrap for multi-line captions
                    c.AppearanceHeader.Options.UseTextOptions = true;
                    c.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                }

                // -------- Conditional formatting (predefined styles) --------
                v.FormatRules.Clear();
                if (rem != null && total != null)
                {
                    // Yellow when Remaining > 0 AND TotalReceivedCases > 0
                    var r1 = new DevExpress.XtraGrid.GridFormatRule { Column = rem, ColumnApplyTo = rem, Name = "Remaining_Pending" };
                    r1.Rule = new DevExpress.XtraEditors.FormatConditionRuleExpression
                    {
                        Expression = "[RemainingCases] > 0 And [TotalReceivedCases] > 0",
                        PredefinedName = "Yellow Fill, Yellow Text"
                    };
                    v.FormatRules.Add(r1);

                    // Green when Remaining == 0 AND TotalReceivedCases > 0
                    var r2 = new DevExpress.XtraGrid.GridFormatRule { Column = rem, ColumnApplyTo = rem, Name = "Remaining_Full" };
                    r2.Rule = new DevExpress.XtraEditors.FormatConditionRuleExpression
                    {
                        Expression = "[RemainingCases] = 0 And [TotalReceivedCases] > 0",
                        PredefinedName = "Green Fill, Green Text"
                    };
                    v.FormatRules.Add(r2);

                    var r3 = new DevExpress.XtraGrid.GridFormatRule { Column = rem, ColumnApplyTo = rem, Name = "Too_Many" };
                    r3.Rule = new DevExpress.XtraEditors.FormatConditionRuleExpression
                    {
                        Expression = "[RemainingCases] < 0",
                        PredefinedName = "Red Fill, Red Text"
                    };
                    v.FormatRules.Add(r3);
                }

                v.OptionsView.ShowBands = true;
                v.BandPanelRowHeight = 28;
            }
            finally
            {
                v.BestFitColumns();
                v.EndUpdate();
            }
        }

        private async Task AddShipmentAsync()
        {
            if (OrderVendorId == 0)
            {
                XtraMessageBox.Show("You must select an order month first.", "Invalid Month", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var dlg = new frmAddShipmentWithItemsDialog(_shipmentRepo, _orderVendorId)
            {
                StartPosition = FormStartPosition.CenterParent
            };

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            using var _ = BusyScope.Show(this);
            await RebuildMatrixAsync();
        }

        // --- Grid wiring (columns, behavior) ---
        private bool _wired;
        private void WireOnce()
        {
            if (_wired) return;
            _wired = true;

            // Ensure we have an AdvBandedGridView attached
            if (viewMatrix is not AdvBandedGridView)
            {
                // If the designer already created AdvBandedGridView named viewMatrix, this block won’t run.
                var banded = new AdvBandedGridView(gridMatrix);
                banded.Appearance.HeaderPanel.FontStyleDelta = FontStyle.Bold;
                gridMatrix.ViewCollection.Add(banded);
                gridMatrix.MainView = banded;
                viewMatrix = banded;
            }

            var v = (AdvBandedGridView)viewMatrix;

            v.BeginUpdate();
            try
            {
                v.Columns.Clear();
                v.Bands.Clear();

                v.OptionsView.ShowGroupPanel = false;
                v.OptionsView.ShowFooter = true;
                v.OptionsBehavior.Editable = false; // read-only matrix
                v.OptionsSelection.MultiSelect = true;
                v.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
                v.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.True; // let headers grow
                v.Appearance.HeaderPanel.Options.UseTextOptions = true;
                v.Appearance.HeaderPanel.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                v.OptionsView.EnableAppearanceEvenRow = true;
                v.ColumnPanelRowHeight = 32;
                v.OptionsView.ColumnAutoWidth = false;
                v.OptionsView.ShowBands = true; // important for AdvBandedGridView

                // Rebuild bands & styles after data binds / columns populate
                v.DataSourceChanged += (_, __) => BuildBandsAndStyle(v);
            }
            finally
            {
                v.EndUpdate();
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            GridExportHelpers.ExportAdvBandedViewToExcel((AdvBandedGridView)gridMatrix.MainView, $"Shipments-{lueOrderMonth.Text}");
        }

        // --- Helper: find the right-most (latest) shipment column ---
        private static DataColumn? FindRightMostShipmentColumn(DataTable dt)
        {
            DataColumn? rightMost = null;
            foreach (DataColumn c in dt.Columns)
            {
                if (c.DataType == typeof(int) && !BaseFields.Contains(c.ColumnName))
                {
                    if (rightMost == null || c.Ordinal > rightMost.Ordinal)
                        rightMost = c;
                }
            }
            return rightMost;
        }

        private static int GetInt(DataRow dr, string col)
        {
            if (!dr.Table.Columns.Contains(col)) return 0;
            var v = dr[col];
            return v == DBNull.Value ? 0 : Convert.ToInt32(v);
        }

        private static decimal GetDecimal(DataRow dr, string col)
        {
            if (!dr.Table.Columns.Contains(col)) return 0m;
            var v = dr[col];
            return v == DBNull.Value ? 0m : Convert.ToDecimal(v);
        }

        // Internal projection just to compute the numbers we need
        private sealed class ShipmentCalcRow
        {
            public int UpcId { get; init; }
            public string UPC { get; init; } = "";
            public string Description { get; init; } = "";
            public decimal Price { get; init; }
            public int QtyOrdered { get; init; }
            public int LatestShipmentQty { get; init; }
            public int RemainingQty { get; init; }
        }

        // Create one pass over the DataTable to compute both figures
        private static IEnumerable<ShipmentCalcRow> EnumerateShipmentCalcRows(DataTable dt)
        {
            var latestCol = FindRightMostShipmentColumn(dt);
            var hasRemainingCol = dt.Columns.Contains("RemainingCases");
            var hasReceivedSumCol = dt.Columns.Contains("TotalReceivedCases");

            foreach (DataRow r in dt.Rows)
            {
                var qtyOrdered = GetInt(r, "QtyInCases");

                int latest = latestCol != null ? GetInt(r, latestCol.ColumnName) : 0;

                int remaining;
                if (hasRemainingCol)
                {
                    remaining = GetInt(r, "RemainingCases");
                    if (remaining < 0) remaining = 0; // clamp if desired
                }
                else
                {
                    // Fallback: compute remaining = ordered - sum(shipments)
                    int sum = 0;
                    foreach (DataColumn c in dt.Columns)
                    {
                        if (c.DataType == typeof(int) && !BaseFields.Contains(c.ColumnName))
                            sum += GetInt(r, c.ColumnName);
                    }
                    remaining = Math.Max(0, qtyOrdered - sum);
                }

                yield return new ShipmentCalcRow
                {
                    UpcId = GetInt(r, "UpcId"),
                    UPC = Convert.ToString(r["UPC"]) ?? "",
                    Description = Convert.ToString(r["Description"]) ?? "",
                    Price = GetDecimal(r, "Price"),
                    QtyOrdered = qtyOrdered,
                    LatestShipmentQty = latest,
                    RemainingQty = remaining
                };
            }
        }

        // Map the projection back into VendorOrderRowVM with QtyInCases set to the
        // scenario-specific quantity (latest shipment or remaining).
        private static VendorOrderRowVM ToVendorOrderRowForQuantity(ShipmentCalcRow s, int qtyForThisExport)
        {
            // Build the domain row with just the fields we need for CSV + safe defaults.
            var r = new VendorOrderGridRow
            {
                // Identity / display
                UpcId = s.UpcId,
                Upc = s.UPC,

                // Snapshot fields that drive CSV
                PriceSnapshot = s.Price,            // decimal
                QtyForThisMonth = qtyForThisExport, // this becomes QtyInCases in the VM ctor
            };

            // VM ctor will set QtyInCases = r.QtyForThisMonth and Price = r.PriceSnapshot
            return new VendorOrderRowVM(r);
        }

        private void btnMainframeCsv_Click(object sender, EventArgs e)
        {
            if (_matrix == null || _matrix.Rows.Count == 0)
            {
                XtraMessageBox.Show(this, "No data to export.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 0) Validate/normalize filenames from the TextEdits (no paths allowed; ensure .csv)
            if (!TryNormalizeCsvFileName(txtFilenameShipment?.Text, out var shipmentFileName, out var err1))
            {
                XtraMessageBox.Show(this, err1, "Invalid filename", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFilenameShipment?.Focus(); txtFilenameShipment?.SelectAll();
                return;
            }
            if (!TryNormalizeCsvFileName(txtFilenameRemaining?.Text, out var remainingFileName, out var err2))
            {
                XtraMessageBox.Show(this, err2, "Invalid filename", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFilenameRemaining?.Focus(); txtFilenameRemaining?.SelectAll();
                return;
            }

            // Optional: reflect normalized names (adds .csv if user omitted it)
            if (txtFilenameShipment != null) txtFilenameShipment.Text = shipmentFileName;
            if (txtFilenameRemaining != null) txtFilenameRemaining.Text = remainingFileName;

            // 1) Compute both quantities once
            var calcRows = EnumerateShipmentCalcRows(_matrix).ToList();

            // 2) Latest-shipment CSV (only rows with a qty in the right-most shipment column)
            var latestRows = calcRows
                .Where(x => x.LatestShipmentQty > 0)
                .Select(x => ToVendorOrderRowForQuantity(x, x.LatestShipmentQty))
                .ToList();

            if (latestRows.Count == 0)
            {
                XtraMessageBox.Show(this, "No items found in the latest shipment.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 3) Remaining-quantity CSV (rows with remaining > 0)
            var remainingRows = calcRows
                .Where(x => x.RemainingQty > 0)
                .Select(x => ToVendorOrderRowForQuantity(x, x.RemainingQty))
                .ToList();

            if (remainingRows.Count == 0)
            {
                XtraMessageBox.Show(this, "No items have remaining quantity.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 4) Ask once for the target folder
            using var folderDlg = new FolderBrowserDialog
            {
                Description = "Choose the folder to save the two CSV files.",
                UseDescriptionForTitle = true,
                ShowNewFolderButton = true,
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (folderDlg.ShowDialog(this) != DialogResult.OK)
                return;

            var shipmentPath = Path.Combine(folderDlg.SelectedPath, shipmentFileName);
            var remainingPath = Path.Combine(folderDlg.SelectedPath, remainingFileName);

            // Prevent accidentally choosing the same name for both files
            if (string.Equals(shipmentPath, remainingPath, StringComparison.OrdinalIgnoreCase))
            {
                XtraMessageBox.Show(this, "The two filenames resolve to the same path. Please use different names.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 5) Confirm overwrites if needed
            var existing = new[]
            {
                File.Exists(shipmentPath)  ? $"• {Path.GetFileName(shipmentPath)}"  : null,
                File.Exists(remainingPath) ? $"• {Path.GetFileName(remainingPath)}" : null}.Where(s => s != null).ToList();

            if (existing.Count > 0)
            {
                var msg = "The following file(s) already exist:\r\n" +
                          string.Join("\r\n", existing) +
                          "\r\n\r\nDo you want to overwrite them?";
                if (XtraMessageBox.Show(this, msg, "Confirm Overwrite", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
            }

            // 6) Write both files
            try
            {
                var csv1 = VendorOrderRowVM.BuildMainframeCsv(latestRows);
                File.WriteAllText(shipmentPath, csv1, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(this, $"Failed to export latest-shipment CSV:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var csv2 = VendorOrderRowVM.BuildMainframeCsv(remainingRows);
                File.WriteAllText(remainingPath, csv2, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(this, $"Failed to export remaining-quantity CSV:\r\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var dr = XtraMessageBox.Show(
                this,
                $"CSV files exported successfully.\n\nOpen containing folder now?",
                "Success",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information
                );

            if (dr == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("explorer.exe", folderDlg.SelectedPath);
            }
        }

        /// <summary>
        /// Validates that the user typed a bare file name (no folder),
        /// ensures it ends with .csv, and rejects invalid/reserved names.
        /// </summary>
        private static bool TryNormalizeCsvFileName(string? input, out string normalized, out string error)
        {
            normalized = string.Empty;
            error = string.Empty;

            var name = (input ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(name))
            {
                error = "Please enter a file name.";
                return false;
            }

            // Must be a file name only (no directory pieces)
            // Using GetInvalidFileNameChars catches slashes and colon as well.
            if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                error = "The file name contains invalid characters. Do not include folder paths.";
                return false;
            }

            // Strongly disallow sneaky paths like ".." or "." as a bare name
            if (name == "." || name == "..")
            {
                error = "Please provide a valid file name, not '.' or '..'.";
                return false;
            }

            // Windows reserved device names (CON, PRN, AUX, NUL, COM1.., LPT1..)
            var baseName = Path.GetFileNameWithoutExtension(name);
            if (IsWindowsReservedName(baseName))
            {
                error = $"'{baseName}' is a reserved name on Windows. Please use a different file name.";
                return false;
            }

            // Force .csv extension (replace any other extension)
            if (!string.Equals(Path.GetExtension(name), ".csv", StringComparison.OrdinalIgnoreCase))
                name = Path.ChangeExtension(name, ".csv")!;

            normalized = name;
            return true;
        }

        private static bool IsWindowsReservedName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;
            var n = name.Trim().ToUpperInvariant();
            if (n is "CON" or "PRN" or "AUX" or "NUL") return true;

            if (n.StartsWith("COM") || n.StartsWith("LPT"))
            {
                // COM1..COM9, LPT1..LPT9
                if (n.Length == 4 && char.IsDigit(n[3]) && n[3] != '0') return true;
            }
            return false;
        }
    }
}
