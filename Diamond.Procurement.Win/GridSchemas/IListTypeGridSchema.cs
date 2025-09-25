using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using Diamond.Procurement.Win.ViewModels; // ← VM namespace

namespace Diamond.Procurement.Win.GridSchemas
{
    public interface IListTypeGridSchema
    {
        void Build(AdvBandedGridView bv, GridControl grid, IReadOnlyList<VendorOrderRowVM> rows, int buyerId);
    }
}
