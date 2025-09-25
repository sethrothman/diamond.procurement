using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using Diamond.Procurement.Win.Helpers;
using Diamond.Procurement.Win.ViewModels; // ← VM
using static Diamond.Procurement.Win.GridSchemas.GridColumnHelpers;

namespace Diamond.Procurement.Win.GridSchemas
{
    public static class GridSchemaShared
    {
        public static BandedGridColumn AddRO(AdvBandedGridView bv, GridBand band, string field, string caption, int? width = null, string? format = null, string? tooltip = null)
        {
            var col = new BandedGridColumn { FieldName = field, Caption = caption, Visible = true, ToolTip = tooltip };
            col.OptionsColumn.AllowEdit = false;
            if (width.HasValue) col.Width = width.Value;
            if (!string.IsNullOrEmpty(format))
            {
                col.DisplayFormat.FormatType = FormatType.Numeric;
                col.DisplayFormat.FormatString = format;
                col.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Far;
            }
            bv.Columns.Add(col);
            band.Columns.Add(col);
            return col;
        }

        public static BandedGridColumn AddRW(AdvBandedGridView bv, GridBand band, string field, string caption, int? width = null, string fmt = "n0", string? tooltip = null)
        {
            var col = new BandedGridColumn { FieldName = field, Caption = caption, Visible = true, ToolTip = tooltip };
            col.OptionsColumn.AllowEdit = true;
            if (width.HasValue) col.Width = width.Value;
            col.DisplayFormat.FormatType = FormatType.Numeric;
            col.DisplayFormat.FormatString = fmt;
            col.AppearanceCell.BackColor = Color.LightYellow;
            col.AppearanceCell.Options.UseBackColor = true;
            col.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Far;
            bv.Columns.Add(col);
            band.Columns.Add(col);
            return col;
        }

        public static GridBand Band(AdvBandedGridView bv, string caption, int? width = null)
        {
            var b = bv.Bands.AddBand(caption);
            if (width.HasValue) b.Width = width.Value;
            b.AppearanceHeader.FontStyleDelta = FontStyle.Bold;
            b.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            b.AppearanceHeader.TextOptions.VAlignment = VertAlignment.Center;
            return b;
        }

        public static void ApplyBaselineViewOptions(AdvBandedGridView bv)
        {
            bv.OptionsBehavior.Editable = true; // RO/RW handled per-column
            bv.OptionsSelection.MultiSelect = true;
            bv.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
            bv.OptionsClipboard.AllowCopy = DefaultBoolean.True;
            bv.OptionsClipboard.CopyColumnHeaders = DefaultBoolean.True;
            bv.OptionsView.ColumnAutoWidth = false;
            bv.OptionsView.ShowGroupPanel = false;
            bv.OptionsView.ShowBands = true;
            bv.OptionsView.ShowFooter = true;
            bv.OptionsView.EnableAppearanceEvenRow = true;
            bv.OptionsFind.FindFilterColumns = "Upc;Description";
            bv.ShowFindPanel();
        }

        public static void BuildCosmeticsLayout(AdvBandedGridView bv, int buyerId)
        {
            bv.Bands.Clear();
            bv.Columns.Clear();
            bv.FormatRules.Clear();

            var bItem = Band(bv, "Item");
            var bVendor = Band(bv, "Vendor");
            var bComps = Band(bv, "Comps");
            bComps.Visible = false;

            var buyerName = FileInference.GetBuyerName(buyerId)!;

            var bBuyer = Band(bv, buyerId == 5 ? "C&S" : buyerName);
            var bMeijer = Band(bv, "Meijer"); bMeijer.Visible = false;
            var bDG = Band(bv, "DG"); bDG.Visible = false;

            var bDiamond = Band(bv, "Diamond");
            var bTotals = Band(bv, "Total");
            var bOrder = Band(bv, "Order Details");

            // Item
            var colUPC = AddRO(bv, bItem, nameof(VendorOrderRowVM.Upc), "UPC", 110);
            AddRO(bv, bItem, nameof(VendorOrderRowVM.Description), "Description", 240);
            AddRO(bv, bItem, nameof(VendorOrderRowVM.CasePack), "Case Pack", 110);
            AddRO(bv, bItem, nameof(VendorOrderRowVM.HI), "HI", 80);
            AddRO(bv, bItem, nameof(VendorOrderRowVM.TI), "TI", 80);

            // Vendor
            AddRO(bv, bVendor, nameof(VendorOrderRowVM.ForecastQtyInCasesVendor), "Forecast", null, "#,0;(#,0);-");
            AddRO(bv, bVendor, nameof(VendorOrderRowVM.Price), "Price", 85, "$#,0.00;(#,0.00);-");

            // Buyer
            AddRO(bv, bBuyer, nameof(VendorOrderRowVM.ForecastQtyInCasesBuyer), "Forecast", null, "#,0;(#,0);-");
            AddRO(bv, bBuyer, nameof(VendorOrderRowVM.WeeklyRunRate), "WRR", null, "#,0.00;(#,0.00);-", "Weekly Run Rate");
            AddRO(bv, bBuyer, nameof(VendorOrderRowVM.AvailableInCasesBuyer), "Inventory", null, "#,0;(#,0);-");
            AddRO(bv, bBuyer, nameof(VendorOrderRowVM.OnPoInCasesBuyer), "On PO", null, "#,0;(#,0);-");
            var colWeeksCoverBuyer = AddRO(bv, bBuyer, nameof(VendorOrderRowVM.WeeksCover), "WOH", null, "#,0.00;(#,0.00);-", "Weeks on Hand");

            var redRule = new GridFormatRule
            {
                Column = colWeeksCoverBuyer,
                Name = "Rule_Gt18Weeks",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = $"([{nameof(VendorOrderRowVM.WeeksCover)}] > 18) AND (IsNull([{nameof(VendorOrderRowVM.ForecastQtyInCasesBuyer)}], 0) >= 1)",
                    PredefinedName = "Red Fill, Red Text"
                }
            };
            bv.FormatRules.Add(redRule);

            // Diamond
            AddRO(bv, bDiamond, nameof(VendorOrderRowVM.AvailableInCasesDiamond), "Inventory", null, "#,0;(#,0);-");
            AddRO(bv, bDiamond, nameof(VendorOrderRowVM.OnPoInCasesDiamond), "On PO", null, "#,0;(#,0);-");
            AddRO(bv, bDiamond, nameof(VendorOrderRowVM.WeeksCoverDiamond), "WOH", null, "#,0.00;(#,0.00);-", "Weeks on Hand");
            AddRO(bv, bDiamond, nameof(VendorOrderRowVM.OverstockDiamond), "Overstock", null, "#,0;(#,0);-");

            // Totals
            if(buyerId == 5)
                AddRO(bv, bTotals, nameof(VendorOrderRowVM.WeeklyRunRateAll), "WRR", null, "#,0.00;(#,0.00);-", "Total Weekly Run Rate");

            AddRO(bv, bTotals, nameof(VendorOrderRowVM.WeeksCoverTotal), "WOH", null, "#,0.00;(#,0.00);-", "Total Weeks on Hand");

            // Order Details
            var proposedCol = AddRO(bv, bOrder, nameof(VendorOrderRowVM.ProposedQtyInCases), "Proposed", 130, "#,0;(#,0);-");
            proposedCol.AppearanceCell.BackColor = Color.FromArgb(216, 234, 255);

            var yellowRule = new GridFormatRule
            {
                Column = proposedCol,
                Name = "Rule_ProposedGtAvailable"
            };
            var yellowExpr = new FormatConditionRuleExpression
            {
                Expression = $"([{nameof(VendorOrderRowVM.ProposedQtyInCases)}] > [{nameof(VendorOrderRowVM.AvailableQtyInCases)}])",
                PredefinedName = "Yellow Fill, Yellow Text"
            };
            yellowRule.Rule = yellowExpr;
            // High priority (must cast to base to access Appearance)
            yellowExpr.Appearance.Options.HighPriority = true;
            bv.FormatRules.Add(yellowRule);

            var availableCol = AddRO(bv, bOrder, nameof(VendorOrderRowVM.AvailableQtyInCases), "Available", 130, "#,0;(#,0);-");
            availableCol.AppearanceCell.BackColor = Color.FromArgb(216, 234, 255);

            var extraCol = AddRW(bv, bOrder, nameof(VendorOrderRowVM.ExtraCases), "Extra Cases", 110, "#,0;(#,0);-");
            var extraTotal = AddRO(bv, bOrder, nameof(VendorOrderRowVM.ExtraLineTotal), "Extended Cost", 120, "$#,0.00;(#,0.00);-");
            extraTotal.AppearanceCell.BackColor = Color.FromArgb(216, 234, 255);

            var qtyCol = AddRW(bv, bOrder, nameof(VendorOrderRowVM.QtyInCases), "Qty to Order", 110, "#,0;(#,0);-");
            var confirmedCol = AddRW(bv, bOrder, nameof(VendorOrderRowVM.QtyConfirmed), "Confirmed", 110, "#,0;(#,0);-");

            var lineTotal = AddRO(bv, bOrder, nameof(VendorOrderRowVM.LineTotal), "Extended Cost", 120, "$#,0.00;(#,0.00);-");
            lineTotal.AppearanceCell.BackColor = Color.FromArgb(216, 234, 255);

            // Summaries
            proposedCol.Summary.Add(new GridColumnSummaryItem(SummaryItemType.Sum, proposedCol.FieldName, "{0:n0}"));
            availableCol.Summary.Add(new GridColumnSummaryItem(SummaryItemType.Sum, availableCol.FieldName, "{0:n0}"));
            extraCol.Summary.Add(new    GridColumnSummaryItem(SummaryItemType.Sum, extraCol.FieldName, "{0:n0}"));
            extraTotal.Summary.Add(new GridColumnSummaryItem(SummaryItemType.Sum, extraTotal.FieldName, "{0:c2}"));
            qtyCol.Summary.Add(new GridColumnSummaryItem(SummaryItemType.Sum, qtyCol.FieldName, "{0:n0}"));
            confirmedCol.Summary.Add(new GridColumnSummaryItem(SummaryItemType.Sum, confirmedCol.FieldName, "{0:n0}"));
            lineTotal.Summary.Add(new GridColumnSummaryItem(SummaryItemType.Sum, lineTotal.FieldName, "{0:c2}"));
            colUPC.Summary.Add(new GridColumnSummaryItem(SummaryItemType.Count, colUPC.FieldName, "{0:n0} Items"));

            // Pin bands
            bItem.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            bOrder.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;

            // Highlight rows where HasAlternateBuyer = true
            var altBuyerRule = new GridFormatRule
            {
                Column = null,             // no specific column, we’ll apply to the whole row
                Name= "Rule_ALtBuyer",
                ApplyToRow = true
            };
            var altBuyerCondition = new FormatConditionRuleExpression
            {
                Expression = $"[{nameof(VendorOrderRowVM.HasAlternateBuyer)}] = True",
                PredefinedName = "Red Fill, Red Text"
            };

            // Ensure this rule outranks other styles
            altBuyerCondition.Appearance.Options.HighPriority = true;
            altBuyerRule.Rule = altBuyerCondition;

            // Add rule to the view
            bv.FormatRules.Add(altBuyerRule);

            // Hide comp fields if VM exposes them (Cosmetics doesn’t use them)
            HideIfExists(bv, nameof(VendorOrderRowVM.TI));
            HideIfExists(bv, nameof(VendorOrderRowVM.HI));

            bv.BestFitColumns();
        }

        public static void PlaceColumnsAtBandStart(GridBand band, params BandedGridColumn[] columnsInDesiredOrder)
        {
            // Put both columns on the first row within the band to avoid “stacking”
            band.RowCount = Math.Max(1, band.RowCount);

            // Remove then insert at the front in reverse to preserve passed order
            for (int i = columnsInDesiredOrder.Length - 1; i >= 0; i--)
            {
                var c = columnsInDesiredOrder[i];
                if (c.OwnerBand != band) c.OwnerBand = band;
                if (band.Columns.Contains(c)) band.Columns.Remove(c);
                c.RowIndex = 0;        // <- keep a single row in the band
                c.Visible = true;
                band.Columns.Insert(0, c);
            }

            // Normalize VisibleIndex based on the band’s column order
            for (int i = 0; i < band.Columns.Count; i++)
                band.Columns[i].VisibleIndex = i;
        }
    }
}
