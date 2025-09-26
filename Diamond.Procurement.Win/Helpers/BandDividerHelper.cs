using System.Linq;
using System.Runtime.CompilerServices;
using DevExpress.XtraGrid.Columns;                     // FixedStyle
using DevExpress.XtraGrid.Views.BandedGrid;           // AdvBandedGridView, GridBand, BandedGridColumn
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;  // BandedGridViewInfo
using DevExpress.XtraGrid.Views.Grid.ViewInfo;        // GridRowInfo

public static class BandDividerHelper
{
    private sealed class State
    {
        public Dictionary<GridBand, int> BandEdgeX = new();   // band -> right-edge X (from band header)
        public int Thickness = 2;
        public Color? LineColor;
        public bool DrawHeader = true;
        public PaintEventHandler? PaintHandler;
    }

    private static readonly ConditionalWeakTable<AdvBandedGridView, State> _states = new();

    public static void Enable(
        AdvBandedGridView view,
        int thickness = 2,
        Color? lineColor = null,
        bool drawHeader = true)
    {
        if (view is null) throw new ArgumentNullException(nameof(view));

        if (!_states.TryGetValue(view, out var state))
        {
            state = new State();
            _states.Add(view, state);

            // Headers
            view.CustomDrawBandHeader += View_CustomDrawBandHeader;
            view.CustomDrawColumnHeader += View_CustomDrawColumnHeader;

            // Rows overlay (after grid paints)
            state.PaintHandler = (s, e) => Grid_Paint(view, e);
            view.GridControl.Paint += state.PaintHandler;
        }

        state.Thickness = Math.Max(1, thickness);
        state.LineColor = lineColor;
        state.DrawHeader = drawHeader;

        view.Invalidate();
    }

    public static void Disable(AdvBandedGridView view)
    {
        if (!_states.TryGetValue(view, out var state)) return;

        view.CustomDrawBandHeader -= View_CustomDrawBandHeader;
        view.CustomDrawColumnHeader -= View_CustomDrawColumnHeader;
        if (state.PaintHandler != null)
            view.GridControl.Paint -= state.PaintHandler;

        _states.Remove(view);
        view.Invalidate();
    }

    // -------------------- ROWS (overlay) --------------------
    private static void Grid_Paint(AdvBandedGridView view, PaintEventArgs e)
    {
        if (!_states.TryGetValue(view, out var state)) return;
        if (view.GetViewInfo() is not BandedGridViewInfo info) return;

        // Data rows span only
        int rowsTop = int.MaxValue, rowsBottom = int.MinValue;
        foreach (GridRowInfo row in info.RowsInfo)
        {
            int h = row.RowHandle;
            if (h < 0 || !view.IsDataRow(h)) continue;
            var r = row.Bounds; if (r.Height <= 0) continue;
            rowsTop = Math.Min(rowsTop, r.Top);
            rowsBottom = Math.Max(rowsBottom, r.Bottom);
        }
        if (rowsTop == int.MaxValue || rowsBottom == int.MinValue) return;

        // Clamp to rows viewport
        var rv = info.ViewRects.Rows;
        if (!rv.IsEmpty)
        {
            rowsTop = Math.Max(rowsTop, rv.Top);
            rowsBottom = Math.Min(rowsBottom, rv.Bottom);
            if (rowsBottom <= rowsTop) return;
        }

        // Get the scrollable area bounds - this is key for the fix
        var scrollableArea = GetScrollableAreaBounds(view, info);
        if (scrollableArea.Width <= 0) return;

        // Build eligible band edges for *this* frame:
        // - have an edge recorded (from column headers this paint)
        // - band is visible, leaf, non-fixed
        // - band currently has at least one visible, non-fixed column
        // - edge position is within the scrollable viewport
        var eligible = new List<(GridBand Band, int X)>();
        foreach (var kv in state.BandEdgeX)
        {
            var band = kv.Key;
            if (band == null || !band.Visible || band.Fixed != FixedStyle.None || band.Children.Count != 0)
                continue;

            bool anyVisibleCol = band.Columns
                .OfType<BandedGridColumn>()
                .Any(c => c.Visible && c.Fixed == FixedStyle.None);
            if (!anyVisibleCol) continue;

            // Only include edges that are within the scrollable area
            int edgeX = kv.Value;
            if (edgeX >= scrollableArea.Left && edgeX <= scrollableArea.Right)
            {
                eligible.Add((band, edgeX));
            }
        }
        if (eligible.Count == 0) return;

        // If the grid exposes a right-fixed pane, DevExpress already paints a separator between
        // the scrollable pane and the fixed pane. Skip the rightmost seam so we don't double it.
        bool hasRightFixed = HasRightFixedPane(view, info);
        if (hasRightFixed)
        {
            int maxEdge = eligible.Max(ed => ed.X);
            eligible.RemoveAll(ed => ed.X >= maxEdge);
            if (eligible.Count == 0) return;
        }

        // Sort by X (viewport order)
        eligible.Sort((a, b) => a.X.CompareTo(b.X));

        // Draw with proper clipping to scrollable area only
        var saved = e.Graphics.Save();
        try
        {
            // Clip to the intersection of data rows and scrollable area
            var clipRect = new Rectangle(
                scrollableArea.Left,
                rowsTop,
                scrollableArea.Width,
                rowsBottom - rowsTop);

            e.Graphics.SetClip(clipRect);

            using var pen = new Pen(state.LineColor ?? view.Appearance.VertLine.ForeColor, 1f);
            foreach (var ed in eligible)
                for (int i = 0; i < state.Thickness; i++)
                    e.Graphics.DrawLine(pen, ed.X - i, rowsTop, ed.X - i, rowsBottom);
        }
        finally { e.Graphics.Restore(saved); }
    }

    // Helper method to get the bounds of the scrollable area (excluding fixed columns)
    private static Rectangle GetScrollableAreaBounds(AdvBandedGridView view, BandedGridViewInfo info)
    {
        var clientRect = view.GridControl.ClientRectangle;
        int left = clientRect.Left;
        int right = clientRect.Right;

        // Get left-fixed columns width
        var leftFixedRect = info.ViewRects.FixedLeft;
        if (!leftFixedRect.IsEmpty)
            left = leftFixedRect.Right;

        // Get right-fixed columns width  
        var rightFixedRect = info.ViewRects.FixedRight;
        if (!rightFixedRect.IsEmpty)
            right = rightFixedRect.Left;

        return new Rectangle(left, clientRect.Top, Math.Max(0, right - left), clientRect.Height);
    }

    // -------------------- HEADERS --------------------

    private static void View_CustomDrawBandHeader(object? sender, BandHeaderCustomDrawEventArgs e)
    {
        var view = (AdvBandedGridView)sender!;
        e.Painter.DrawObject(e.Info); // default band header

        if (!_states.TryGetValue(view, out var state) || !state.DrawHeader) { e.Handled = true; return; }
        var band = e.Band;
        if (band == null || !band.Visible || band.Fixed != FixedStyle.None || band.Children.Count != 0)
        {   // only leaf, non-fixed bands
            e.Handled = true; return;
        }

        // Use the per-frame edge captured from column headers
        if (!state.BandEdgeX.TryGetValue(band, out int x0)) { e.Handled = true; return; }

        if (view.GetViewInfo() is BandedGridViewInfo info)
        {
            var scrollableArea = GetScrollableAreaBounds(view, info);
            if (x0 < scrollableArea.Left || x0 > scrollableArea.Right)
            {
                e.Handled = true;
                return;
            }

            if (IsRightmostNonFixedEdge(view, state, band, x0, info)) { e.Handled = true; return; }
        }
        else if (IsRightmostNonFixedEdge(view, state, band, x0, null)) { e.Handled = true; return; }

        using var pen = new Pen(state.LineColor ?? view.Appearance.VertLine.ForeColor, 1f);
        for (int i = 0; i < state.Thickness; i++)
            e.Graphics.DrawLine(pen, x0 - i, e.Bounds.Top, x0 - i, e.Bounds.Bottom);

        e.Handled = true;
    }

    private static bool IsRightmostNonFixedEdge(AdvBandedGridView view, State state, GridBand candidateBand, int candidateX, BandedGridViewInfo? info)
    {
        bool hasRightFixed = false;
        Rectangle scrollableArea = Rectangle.Empty;

        if (info != null)
        {
            hasRightFixed = HasRightFixedPane(view, info);
            scrollableArea = GetScrollableAreaBounds(view, info);
        }

        if (!hasRightFixed)
        {
            hasRightFixed = view.Columns.OfType<BandedGridColumn>()
                .Any(c => c.Visible && c.Fixed == FixedStyle.Right);
            if (!hasRightFixed) return false;
        }

        // Compute the max X among visible, leaf, non-fixed bands that have edges this frame
        // and are within the scrollable area
        int maxX = int.MinValue;
        foreach (var kv in state.BandEdgeX)
        {
            var b = kv.Key;
            if (b == null || !b.Visible || b.Fixed != FixedStyle.None || b.Children.Count != 0) continue;

            bool anyVisibleCol = b.Columns.OfType<BandedGridColumn>()
                .Any(c => c.Visible && c.Fixed == FixedStyle.None);
            if (!anyVisibleCol) continue;

            // Only consider edges within scrollable area
            int edgeX = kv.Value;
            if (!scrollableArea.IsEmpty && (edgeX < scrollableArea.Left || edgeX > scrollableArea.Right))
                continue;

            if (edgeX > maxX) maxX = edgeX;
        }
        if (maxX == int.MinValue) return false;

        return candidateX >= maxX; // treat ties as rightmost
    }

    private static void View_CustomDrawColumnHeader(object? sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
    {
        var view = (AdvBandedGridView)sender!;
        e.Painter.DrawObject(e.Info); // paint the column header normally

        if (!_states.TryGetValue(view, out var state) || !state.DrawHeader) { e.Handled = true; return; }

        if (e.Column is not BandedGridColumn col) { e.Handled = true; return; }
        if (!col.Visible || col.Fixed != DevExpress.XtraGrid.Columns.FixedStyle.None) { e.Handled = true; return; }

        var band = col.OwnerBand;
        if (band == null || !band.Visible || band.Fixed != FixedStyle.None || band.Children.Count != 0)
        {   // only leaf, non-fixed bands
            e.Handled = true; return;
        }

        // Record the right edge for this band based on the right-most column header we see this frame
        int cx = e.Bounds.Right - 1;
        if (!state.BandEdgeX.TryGetValue(band, out int prev) || cx > prev)
            state.BandEdgeX[band] = cx;

        // Draw the divider ONLY on the last column in the band (where the column's right edge equals the band's edge),
        // and skip it if this seam is the rightmost non-fixed seam when a Right-fixed pane exists.
        if (!state.BandEdgeX.TryGetValue(band, out int x0)) { e.Handled = true; return; }
        if (cx != x0) { e.Handled = true; return; }
        if (view.GetViewInfo() is BandedGridViewInfo info)
        {
            var scrollableArea = GetScrollableAreaBounds(view, info);
            if (x0 < scrollableArea.Left || x0 > scrollableArea.Right)
            {
                e.Handled = true;
                return;
            }

            if (IsRightmostNonFixedEdge(view, state, band, x0, info)) { e.Handled = true; return; }
        }
        else if (IsRightmostNonFixedEdge(view, state, band, x0, null)) { e.Handled = true; return; }

        using var pen = new Pen(state.LineColor ?? view.Appearance.VertLine.ForeColor, 1f);
        for (int i = 0; i < state.Thickness; i++)
            e.Graphics.DrawLine(pen, x0 - i, e.Bounds.Top, x0 - i, e.Bounds.Bottom);

        e.Handled = true;
    }

    private static bool HasRightFixedPane(AdvBandedGridView view, BandedGridViewInfo info)
    {
        if (!info.ViewRects.FixedRight.IsEmpty)
            return true;

        return view.Columns.OfType<BandedGridColumn>()
            .Any(c => c.Visible && c.Fixed == FixedStyle.Right);
    }

}
