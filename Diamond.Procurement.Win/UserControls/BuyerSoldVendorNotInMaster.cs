using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Diamond.Procurement.Data;
using Diamond.Procurement.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Diamond.Procurement.Win.UserControls
{
    public partial class BuyerSoldVendorNotInMaster : DevExpress.XtraEditors.XtraUserControl
    {
        public required BuyerInventoryRepository _repo;
        private readonly BindingSource _bs;

        public BuyerSoldVendorNotInMaster()
        {
            InitializeComponent();

            _bs = new BindingSource();
            gridControl1.DataSource = _bs;

            ConfigureGrid();
            BuildColumns();
        }

        public void Configure(BuyerInventoryRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public async Task InitializeAsync(CancellationToken ct = default)
        {
            if (_repo == null)
                throw new InvalidOperationException("Call Configure(...) before InitializeAsync().");

            await LoadGridAsync(ct);
        }

        private async Task LoadGridAsync(CancellationToken ct)
        {
            UseWaitCursor = true;
            try
            {
                var rows = await _repo.ListBuyerSoldVendorNotInMasterAsync(ct);
                _bs.DataSource = new BindingList<BuyerSoldVendorNotInMasterRow>(new List<BuyerSoldVendorNotInMasterRow>(rows));

                gridView1.BestFitColumns();
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void ConfigureGrid()
        {
            gridView1.OptionsBehavior.Editable = false;
            gridView1.OptionsSelection.MultiSelect = true;
            gridView1.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
            gridView1.OptionsView.ShowGroupPanel = false;
            gridView1.OptionsView.ColumnAutoWidth = false;            // keep like your other pages
            gridView1.OptionsFind.AlwaysVisible = true;               // search panel
            gridView1.OptionsClipboard.AllowCopy = DevExpress.Utils.DefaultBoolean.True;
            gridView1.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.True;
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.BestFitMaxRowCount = 100;
            gridView1.Appearance.HeaderPanel.FontStyleDelta = FontStyle.Bold;
        }

        private void BuildColumns()
        {
            gridView1.Columns.Clear();

            var cUpc = gridView1.Columns.AddVisible(nameof(BuyerSoldVendorNotInMasterRow.Upc), "UPC");
            cUpc.Width = 130;
            cUpc.Summary.Add(new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, cUpc.FieldName, "Items: {0:n0}"));

            var cDesc = gridView1.Columns.AddVisible(nameof(BuyerSoldVendorNotInMasterRow.Description), "Description");
            cDesc.Width = 300;

            var cBuyerInv = gridView1.Columns.AddVisible(nameof(BuyerSoldVendorNotInMasterRow.BuyerInventory), "Buyer Inventory");
            cBuyerInv.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cBuyerInv.DisplayFormat.FormatString = "n0";
            cBuyerInv.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            cBuyerInv.Width = 160;

            var cYTD = gridView1.Columns.AddVisible(nameof(BuyerSoldVendorNotInMasterRow.SoldYtd), "Sold YTD");
            cYTD.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cYTD.DisplayFormat.FormatString = "n0";
            cYTD.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            cYTD.Width = 140;

            var cLY = gridView1.Columns.AddVisible(nameof(BuyerSoldVendorNotInMasterRow.SoldLastYear), "Sold Last Year");
            cLY.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cLY.DisplayFormat.FormatString = "n0";
            cLY.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            cLY.Width = 160;

            var cVQ = gridView1.Columns.AddVisible(nameof(BuyerSoldVendorNotInMasterRow.VendorQty), "Vendor Qty");
            cLY.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cLY.DisplayFormat.FormatString = "n0";
            cLY.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            cLY.Width = 160;
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadGridAsync(CancellationToken.None);
        }
    }
}
