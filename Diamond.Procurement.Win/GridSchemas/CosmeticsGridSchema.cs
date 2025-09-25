using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using Diamond.Procurement.Domain.Models;
using Diamond.Procurement.Win.ViewModels;
using System.Collections.Generic;

namespace Diamond.Procurement.Win.GridSchemas
{
    /// Cosmetics = exactly today’s WireOnce (comp columns hidden).
    public sealed class CosmeticsGridSchema : IListTypeGridSchema
    {
        public void Build(AdvBandedGridView bv, GridControl grid, IReadOnlyList<VendorOrderRowVM> rows, int buyerId)
        {
            grid.DataSource = rows;
            GridSchemaShared.ApplyBaselineViewOptions(bv);
            GridSchemaShared.BuildCosmeticsLayout(bv, buyerId);
        }
    }
}
