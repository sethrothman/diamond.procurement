
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using Diamond.Procurement.Win.ViewModels;
using static Diamond.Procurement.Win.GridSchemas.GridColumnHelpers;


namespace Diamond.Procurement.Win.GridSchemas
{
    public sealed class HaircareGridSchema : IListTypeGridSchema
    {
        // simple in-memory storage for editable unbound column (WeeksToBuy), keyed by UpcId
        //private readonly Dictionary<int, int> _weeksToBuy = [];

        public void Build(AdvBandedGridView bv, GridControl grid, IReadOnlyList<VendorOrderRowVM> rows, int buyerId)
        {
            grid.DataSource = rows;
            GridSchemaShared.ApplyBaselineViewOptions(bv);

            bv.BeginUpdate();
            try
            {
                // Start with Cosmetics layout
                GridSchemaShared.BuildCosmeticsLayout(bv, buyerId);

                // Bands we’ll target
                var vendorBand = GetBand(bv, "Vendor")!;
                var itemBand = GetBand(bv, "Item")!;

                // 1) Competition band right after Vendor
                var compBand = GetBand(bv, "Comps")!;
                compBand.Visible = true;

                // 2) ListPrice in Item band, right AFTER CasePack (bound RO column)
                var listPriceCol = GridSchemaShared.AddRO(bv, itemBand, nameof(VendorOrderRowVM.ListPrice), "List Price", 85, "$#,0.00;(#,0.00);-");
                PlaceColumnAfter(bv, listPriceCol, itemBand, nameof(VendorOrderRowVM.CasePack));

                // 3) Off List % (unbound RO) RIGHT AFTER ListPrice in Item band
                var discountPct = new BandedGridColumn
                {
                    Caption = "Off List %",
                    FieldName = "DiscountPct", // virtual (unbound)
                    UnboundType = UnboundColumnType.Decimal,
                    UnboundExpression = "Iif(IsNullOrEmpty([ListPrice]) Or [ListPrice] = 0, Null, 1 - ([Price] / [ListPrice]))",
                    Visible = true
                };
                discountPct.DisplayFormat.FormatType = FormatType.Numeric;
                discountPct.DisplayFormat.FormatString = "p2";
                discountPct.OptionsColumn.AllowEdit = false;

                bv.Columns.Add(discountPct);
                PlaceColumnAfter(bv, discountPct, vendorBand, nameof(VendorOrderRowVM.Price));

                // 4) ADD Haircare comp columns (RO) into Competition band, then order them
                // PriceVictory ($), QtyVictory, PriceQualityKing ($), QtyQualityKing
                var compPriceVictory = GridSchemaShared.AddRO(bv, compBand, nameof(VendorOrderRowVM.PriceVictory), "Victory", 80, "$#,0.00;(#,0.00);-");
                var compQtyVictory = GridSchemaShared.AddRO(bv, compBand, nameof(VendorOrderRowVM.QtyVictory), "Qty", 70, "#,0;(#,0);-");
                var compPriceQualityKing = GridSchemaShared.AddRO(bv, compBand, nameof(VendorOrderRowVM.PriceQualityKing), "Quality King", 95, "$#,0.00;(#,0.00);-");
                var compQtyQualityKing = GridSchemaShared.AddRO(bv, compBand, nameof(VendorOrderRowVM.QtyQualityKing), "Qty", 95, "#,0;(#,0);-");

                // Deterministic order within Competition band
                compPriceVictory.VisibleIndex = 0;
                compQtyVictory.VisibleIndex = 1;
                compPriceQualityKing.VisibleIndex = 2;
                compQtyQualityKing.VisibleIndex = 3;

                // 5) GP vs. Comp = 1 - (Price / min(PriceVictory, PriceQualityKing))
                var bestComp = new BandedGridColumn
                {
                    Caption = "GP vs. Comp",
                    FieldName = "BestCompDiscount", // virtual (unbound)
                    UnboundType = UnboundColumnType.Decimal,
                    UnboundExpression =
                        "Iif(([PriceVictory]=0 And [PriceQualityKing]=0) Or [Price] Is Null Or [Price]=0," +
                        " 0, 1 - ([Price] / Iif([PriceVictory]=0, [PriceQualityKing], Iif([PriceQualityKing]=0, [PriceVictory], Min([PriceVictory],[PriceQualityKing])))))",
                    Visible = true
                };
                bestComp.DisplayFormat.FormatType = FormatType.Numeric;
                bestComp.DisplayFormat.FormatString = $"#.00%;;No Comp";
                bestComp.OptionsColumn.AllowEdit = false;

                bv.Columns.Add(bestComp);
                PlaceColumnAfter(bv, bestComp, compBand, nameof(VendorOrderRowVM.QtyQualityKing));

                var ruleGpVsCompLow = new GridFormatRule
                {
                    Column = bestComp,
                    Name = "Rule_GpVsComp_Low"
                };
                var exprGpVsCompLow = new FormatConditionRuleExpression
                {
                    // BestCompDiscount is a fraction (0.10 == 10%)
                    Expression = $"[{bestComp.FieldName}] > 0 And ([{bestComp.FieldName}] < 0.10)",
                    PredefinedName = "Red Fill, Red Text"
                };
                ruleGpVsCompLow.Rule = exprGpVsCompLow;
                bv.FormatRules.Add(ruleGpVsCompLow);

                //var bBuyer = GetBand(bv, "C&S");
                var bMeijer = GetBand(bv, "Meijer")!;
                bMeijer.Visible = true;
                GridSchemaShared.AddRO(bv, bMeijer, nameof(VendorOrderRowVM.MeijerWeeklyRunRate), "WRR", 85, "#,0.00;(#,0.00);-", "Weekly Run Rate");
                GridSchemaShared.AddRO(bv, bMeijer, nameof(VendorOrderRowVM.MeijerStrikePrice), "Strike", 85, "$#,0.00;($#,0.00);-");

                var bDG = GetBand(bv, "DG")!;
                bDG.Visible = true;
                GridSchemaShared.AddRO(bv, bDG, nameof(VendorOrderRowVM.DGWeeklyRunRate), "WRR", 85, "#,0.00;(#,0.00);-", "Weekly Run Rate");
                GridSchemaShared.AddRO(bv, bDG, nameof(VendorOrderRowVM.DGStrikePrice), "Strike", 85, "$#,0.00;($#,0.00);-");

                // 6) Editable Weeks To Buy (unbound int) BEFORE Qty to Order in Order Details band
                var orderBand = GetBand(bv, "Order Details")!;
                var weeksToBuyCol = GridSchemaShared.AddRW(bv, orderBand, nameof(VendorOrderRowVM.WeeksToBuy), "Weeks to Buy", 85, "#,0;(#,0);-");
                weeksToBuyCol.Summary.Add(new GridColumnSummaryItem(SummaryItemType.Sum, weeksToBuyCol.FieldName, "{0:n0}"));

                var proposedCol = GridSchemaShared.AddRO(bv, orderBand, nameof(VendorOrderRowVM.ProposedQtyFromWeeks), "Proposed", 85, "#,0;(#,0);-");
                proposedCol.AppearanceCell.BackColor = Color.FromArgb(216, 234, 255);
                proposedCol.Summary.Add(new GridColumnSummaryItem(SummaryItemType.Sum, proposedCol.FieldName, "{0:n0}"));

                var palletCountCol = GridSchemaShared.AddRO(bv, orderBand, nameof(VendorOrderRowVM.PalletCountActive), "Pallet Count", 85, "#,0.00;(#,0.00);-");
                palletCountCol.AppearanceCell.BackColor = Color.FromArgb(216, 234, 255);
                palletCountCol.Summary.Add(new GridColumnSummaryItem(SummaryItemType.Sum, palletCountCol.FieldName, "{0:n2}"));

                var lineTotalProposedCol = GridSchemaShared.AddRO(bv, orderBand, nameof(VendorOrderRowVM.LineTotalFromWeeks), "Extended Cost", 85, "$#,0.00;(#,0.00);-");
                lineTotalProposedCol.AppearanceCell.BackColor = Color.FromArgb(216, 234, 255);
                lineTotalProposedCol.Summary.Add(new GridColumnSummaryItem(SummaryItemType.Sum, lineTotalProposedCol.FieldName, "{0:c2}"));
                lineTotalProposedCol.AppearanceCell.BackColor = Color.FromArgb(216, 234, 255);

                GridSchemaShared.PlaceColumnsAtBandStart(orderBand, weeksToBuyCol, proposedCol, palletCountCol, lineTotalProposedCol);

                // Hide Cosmetics-only columns in Haircare (your list)
                HideIfExists(bv, nameof(VendorOrderRowVM.AvailableQtyInCases));
                HideIfExists(bv, nameof(VendorOrderRowVM.ForecastQtyInCasesVendor));
                HideIfExists(bv, nameof(VendorOrderRowVM.ForecastQtyInCasesBuyer));
                HideIfExists(bv, nameof(VendorOrderRowVM.ProposedQtyInCases));
                HideIfExists(bv, nameof(VendorOrderRowVM.LineTotalDesired));
                HideIfExists(bv, nameof(VendorOrderRowVM.ExtraCases));
                HideIfExists(bv, nameof(VendorOrderRowVM.ExtraLineTotal));

                ShowIfExists(bv, nameof(VendorOrderRowVM.HI));
                ShowIfExists(bv, nameof(VendorOrderRowVM.TI));
            }
            finally
            {
                bv.EndUpdate();
                bv.BestFitColumns();
            }
        }

        private void Bv_CustomColumnDisplayText_NoComp(object? sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column?.FieldName == "BestCompDiscount" && (e.Value == null || e.Value == DBNull.Value))
                e.DisplayText = "No Comp";
        }
    }
}
