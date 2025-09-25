using System.Linq;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;

namespace Diamond.Procurement.Win.GridSchemas
{
    internal static class GridColumnHelpers
    {
        // ------- Generic getters -------
        public static BandedGridColumn? Get(AdvBandedGridView bv, string field)
            => bv.Columns.ColumnByFieldName(field) as BandedGridColumn;

        public static GridBand? GetBand(AdvBandedGridView bv, string caption)
            => bv.Bands.Cast<GridBand>().FirstOrDefault(b => b.Caption == caption);

        // ------- Hide/Show -------
        public static void HideIfExists(AdvBandedGridView bv, string field)
        {
            var c = Get(bv, field);
            if (c != null) c.Visible = false;
        }
        public static void ShowIfExists(AdvBandedGridView bv, string field)
        {
            var c = Get(bv, field);
            if (c != null) c.Visible = true;
        }

        // ------- Band placement -------
        public static void InsertBandAfter(AdvBandedGridView bv, GridBand band, GridBand afterBand)
        {
            // 1) Detach from current parent collection
            if (band.ParentBand != null)
                band.ParentBand.Children.Remove(band);
            else
                bv.Bands.Remove(band);

            // 2) Attach to the SAME parent collection as afterBand
            var parent = afterBand.ParentBand;
            if (parent != null)
                parent.Children.Add(band);   // child band
            else
                bv.Bands.Add(band);          // top-level band

            // Align Fixed group so it renders in same group (Left/Right/None)
            band.Fixed = afterBand.Fixed;

            // 3) Reorder siblings deterministically
            var siblings = (parent != null ? parent.Children : bv.Bands)
                .Cast<GridBand>()
                .Where(b => b.Fixed == afterBand.Fixed) // keep within same fixed group
                .OrderBy(b => b.VisibleIndex)
                .ToList();

            // Ensure our band is part of the sibling set
            if (!siblings.Contains(band))
                siblings.Add(band);

            // Place immediately after the reference band
            var idx = siblings.IndexOf(afterBand);
            if (idx < 0) idx = siblings.Count - 2; // safety fallback
            siblings.Remove(band);
            siblings.Insert(System.Math.Min(idx + 1, siblings.Count), band);

            // Re-assign VisibleIndex across the sibling set
            for (int i = 0; i < siblings.Count; i++)
                siblings[i].VisibleIndex = i;
        }

        public static void InsertBandBefore(GridBand band, GridBand beforeBand)
            => band.VisibleIndex = beforeBand.VisibleIndex;

        // ------- Column placement inside a band -------
        // Places 'col' into 'band' immediately AFTER the reference column (by field name).
        public static void PlaceColumnAfter(AdvBandedGridView bv, BandedGridColumn col, GridBand band, string refField)
        {
            var refCol = Get(bv, refField);
            col.OwnerBand = band;
            col.Visible = true;

            if (refCol != null && refCol.OwnerBand == band)
                col.VisibleIndex = refCol.VisibleIndex + 1;
            else
                col.VisibleIndex = band.Columns.Count - 1; // last in the band
        }

        // Places 'col' into 'band' immediately BEFORE the reference column (by field name).
        public static void PlaceColumnBefore(AdvBandedGridView bv, BandedGridColumn col, GridBand band, string refField)
        {
            var refCol = Get(bv, refField);
            col.OwnerBand = band;
            col.Visible = true;

            if (refCol != null && refCol.OwnerBand == band)
                col.VisibleIndex = refCol.VisibleIndex;  // will shift refCol to the right
            else
                col.VisibleIndex = 0; // first in the band
        }
    }
}
