using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using Diamond.Procurement.Data;
using Diamond.Procurement.Data.Contracts;
using Diamond.Procurement.Win.Forms;
using Diamond.Procurement.Win.ViewModels;
using System.ComponentModel;

namespace Diamond.Procurement.Win.UserControls
{
    public partial class MasterListManagePage : XtraUserControl
    {
        private IMasterListRepository? _repo;

        // Bindings
        private readonly BindingList<MasterListItemVM> _rows = [];
        private BandedGridColumn? _colHasAlt;

        // Page context
        private int _masterListId;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MasterListId
        {
            get => _masterListId;
            set => _masterListId = value;
        }

        public MasterListManagePage()
        {
            InitializeComponent();

            gridControl1.DataSource = _rows;

            // simple two-way binding (like your Analysis page)
            lueMasterList.DataBindings.Add("EditValue", this, nameof(MasterListId),
                true, DataSourceUpdateMode.OnPropertyChanged, 0);

            // wire click handlers
            btnRefreshData.Click += async (_, __) => await LoadGridAsync();
            btnBulkPaste.Click += async (_, __) => await BulkPasteAsync();
            btnAddSingle.Click += async (_, __) => await AddSingleUpcAsync();
            btnRemoveSelected.Click += (_, __) => MarkSelectedForRemoval();

            barManager1.SetPopupContextMenu(gridControl1, popupMenu1);

            popupMenu1.BeforePopup += (_, __) =>
            {
                var gv = (GridView)gridControl1.MainView;
                bbiDiscontinued.Enabled = gv.GetSelectedRows().Length > 0;
            };

            btnSave.Click += async (_, __) => await SaveAltBuyerChangesAsync();
        }

        public void Configure(IMasterListRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public async Task InitializeAsync(CancellationToken ct = default)
        {
            WireOnce();

            // lists for the LookUp
            var lists = await _repo!.GetAllMasterLists(ct); // returns List<MasterListSummary>
            lueMasterList.Properties.DataSource = lists;
            lueMasterList.Properties.DisplayMember = "Name";
            lueMasterList.Properties.ValueMember = "MasterListId";
            lueMasterList.ItemIndex = 0;

            lueMasterList.EditValueChanged += async (_, __) => await LoadGridAsync();

            await LoadGridAsync();
        }

        private void WireOnce()
        {
            if (gridControl1.MainView is AdvBandedGridView) return;

            var bv = new AdvBandedGridView(gridControl1);
            gridControl1.ViewCollection.Add(bv);
            gridControl1.MainView = bv;

            // Allow editing, but lock down columns by default
            bv.OptionsBehavior.Editable = true;
            bv.OptionsSelection.MultiSelect = true;
            bv.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
            bv.OptionsView.ColumnAutoWidth = false;
            bv.OptionsView.ShowGroupPanel = false;
            bv.OptionsView.ShowBands = true;
            bv.OptionsView.ShowFooter = true;
            bv.OptionsView.EnableAppearanceEvenRow = true;
            bv.ShowFindPanel();

            var bItem = Band(bv, "Item");
            var bState = Band(bv, "State");

            var colUpc = AddCol(bv, bItem, nameof(MasterListItemVM.Upc), "UPC", 120);
            colUpc.Summary.Add(new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Count, colUpc.FieldName, "{0:n0} Items"));

            AddCol(bv, bItem, nameof(MasterListItemVM.Description), "Description", 260);
            AddCol(bv, bState, nameof(MasterListItemVM.IsActive), "Active?", 60).OptionsColumn.AllowEdit = false;
            AddCol(bv, bState, nameof(MasterListItemVM.DateAdded), "Date Added", 95, "d").OptionsColumn.AllowEdit = false;
            AddCol(bv, bState, nameof(MasterListItemVM.DateRemoved), "Date Discontinued", 95, "d").OptionsColumn.AllowEdit = false;

            // ✔ editable checkbox column
            _colHasAlt = AddCol(bv, bState, nameof(MasterListItemVM.HasAlternateBuyer), "Has Alternate Buyer?", 80);
            _colHasAlt.OptionsColumn.AllowEdit = true;
            _colHasAlt.OptionsColumn.ReadOnly = false;

            var check = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            gridControl1.RepositoryItems.Add(check);
            _colHasAlt.ColumnEdit = check;

            // Commit editor changes immediately so CellValueChanged fires reliably
            check.EditValueChanged += (_, __) =>
            {
                bv.PostEditor();
                bv.UpdateCurrentRow();
            };

            // Optional: visually mark dirty rows (e.g., italics)
            bv.RowStyle += (s, e) =>
            {
                var row = bv.GetRow(e.RowHandle) as MasterListItemVM;
                if (row?.IsAltBuyerDirty == true)
                {
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Italic);
                }
            };

            bItem.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
        }

        GridBand Band(AdvBandedGridView bv, string caption, int? width = null)
        {
            var b = bv.Bands.AddBand(caption);
            if (width.HasValue)
                b.Width = width.Value;

            b.AppearanceHeader.FontStyleDelta = FontStyle.Bold;
            b.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            b.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;

            return b;
        }

        private static BandedGridColumn AddCol(AdvBandedGridView bv, GridBand band, string field, string caption, int width, string? format = null)
        {
            var c = new BandedGridColumn { FieldName = field, Caption = caption, Visible = true, Width = width };
            if (!string.IsNullOrEmpty(format))
            {
                c.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                c.DisplayFormat.FormatString = format;
            }

            c.OptionsColumn.AllowEdit = false;
            c.OptionsColumn.ReadOnly = true;

            bv.Columns.Add(c);
            band.Columns.Add(c);
            return c;
        }

        private async Task LoadGridAsync()
        {
            if (_repo is null || MasterListId <= 0) return;

            var gv = (GridView)gridControl1.MainView;
            gv.BeginUpdate();
            try
            {
                _rows.Clear();
                var data = await _repo.GetDetailsAsync(MasterListId, CancellationToken.None);
                foreach (var r in data)
                {
                    var vm = new MasterListItemVM(r);
                    _rows.Add(vm);
                }
            }
            finally
            {
                gv.BestFitColumns();
                gv.EndUpdate();
            }
        }

        private void MarkSelectedForRemoval()
        {
            var gv = (GridView)gridControl1.MainView;
            var selected = gv.GetSelectedRows()
                             .Select(h => gv.GetRow(h) as MasterListItemVM)
                             .Where(x => x != null && x.IsActive)
                             .ToList()!;
            if (selected.Count == 0) return;

            var confirm = XtraMessageBox.Show(this,
                $"Mark {selected.Count:n0} UPC(s) discontinued?",
                "Confirm Action",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.No) return;

            // Apply immediately via repository
            var upcs = selected.Select(s => s.Upc!).ToList();
            _ = RemoveAsync(upcs); // fire/forget + refresh
        }

        private async Task RemoveAsync(List<string> upcs)
        {
            try
            {
                await _repo!.BulkApplyAsync(MasterListId, upcsToAdd: Array.Empty<string>(), upcsToRemove: upcs, CancellationToken.None);
                await LoadGridAsync();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(this, $"Remove failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task AddSingleUpcAsync()
        {
            var upc = XtraInputBox.Show("Enter UPC to add:", "Add UPC", "");
            if (string.IsNullOrWhiteSpace(upc)) return;
            await ApplyBulkAsync([upc], Array.Empty<string>());
        }

        private async Task BulkPasteAsync()
        {
            using var dlg = new frmBulkPasteUpcs();

            // Find the parent form that contains this user control
            var owner = this.FindForm();

            // Make the dialog center on that form
            dlg.StartPosition = FormStartPosition.CenterParent;

            if (dlg.ShowDialog(owner) != DialogResult.OK)
                return;

            await ApplyBulkAsync(dlg.UpcsToAdd, dlg.UpcsToRemove);
        }

        private async Task ApplyBulkAsync(IEnumerable<string> add, IEnumerable<string> remove)
        {
            var addList = add?.ToList() ?? new();
            var removeList = remove?.ToList() ?? new();

            if (addList.Count == 0 && removeList.Count == 0)
            {
                XtraMessageBox.Show(this, "Nothing to apply.", "Bulk Paste", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                await _repo!.BulkApplyAsync(MasterListId, addList, removeList, CancellationToken.None);
                await LoadGridAsync();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(this, $"Bulk apply failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task SaveAltBuyerChangesAsync()
        {
            var dirty = _rows.Where(r => r.IsAltBuyerDirty).ToList();
            if (dirty.Count == 0)
            {
                XtraMessageBox.Show(this, "No alternate-buyer changes to save.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                await _repo!.MarkAsAlternateBuyerPerRowAsync(dirty.Select(d => new AlternateBuyerRow(d.MasterListDetailId, d.HasAlternateBuyer)), CancellationToken.None);

                // refresh or just clear dirty + repaint
                foreach (var r in dirty) r.ClearDirty();
                await LoadGridAsync(); // if you prefer server truth

                gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(this, $"Save failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
