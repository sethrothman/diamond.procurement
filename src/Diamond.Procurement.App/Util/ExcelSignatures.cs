using System;
using ClosedXML.Excel;

namespace Diamond.Procurement.Domain.Util
{
    public static class ExcelSignatures
    {
        // -----------------------
        // CNS Inventory
        // -----------------------
        public sealed class CnsInventoryHeaderMap
        {
            public int HeaderRow { get; init; }
            public int Upc { get; init; }
            public int Description { get; init; }
            public int MasterCase { get; init; }
            public int Boh { get; init; }
            public int OnOrder { get; init; }
            public int Avg13 { get; init; }
            public int LocationName { get; init; }   // optional in IsValid
            public int Pack { get; init; }           // optional in IsValid
            public int Mos { get; init; }

            // Required: HeaderRow, Upc, Description, MasterCase, Boh, OnOrder, Avg13, Mos
            public bool IsValid =>
                HeaderRow > 0 &&
                Upc > 0 &&
                Description > 0 &&
                MasterCase > 0 &&
                Boh > 0 &&
                OnOrder > 0 &&
                Avg13 > 0 &&
                Mos > 0;
        }

        public static bool TryMapCnsInventory(IXLWorksheet ws, out CnsInventoryHeaderMap map)
        {
            map = new CnsInventoryHeaderMap();

            // Some CNS files have a banner row; scan a few rows to find the header.
            var lastHeaderRowToScan = Math.Min(5, ws.LastRowUsed()?.RowNumber() ?? 1);

            for (int r = 1; r <= lastHeaderRowToScan; r++)
            {
                try
                {
                    // Required columns
                    var upc = ExcelHeader.Find(ws, r, "UPC (VEN-ITEM)", "UPC (VEN ITEM)", "UPC(VEN-ITEM)", "UPC VEN-ITEM", "UPC");
                    var desc = ExcelHeader.Find(ws, r, "Description");
                    var masterCase = ExcelHeader.Find(ws, r, "MasterCase", "Master Case");
                    var boh = ExcelHeader.Find(ws, r, "BOH", "On Hand", "Total On Hand", "OnHand");
                    var onOrder = ExcelHeader.Find(ws, r, "Total On Order", "On Order", "On PO", "OnPO");
                    var avg13 = ExcelHeader.Find(ws, r, "13 Week Avg Mvmnt", "13 Wk Avg Mvmnt", "13 Week Average Movement", "13 Week Avg Movement");
                    var mos = ExcelHeader.Find(ws, r, "MOS");

                    // Optional columns
                    ExcelHeader.TryFind(ws, r, out var location, "Location Name", "Location");
                    ExcelHeader.TryFind(ws, r, out var pack, "Pack");

                    var candidate = new CnsInventoryHeaderMap
                    {
                        HeaderRow = r,
                        Upc = upc,
                        Description = desc,
                        MasterCase = masterCase,
                        Boh = boh,
                        OnOrder = onOrder,
                        Avg13 = avg13,
                        LocationName = location,
                        Pack = pack,
                        Mos = mos
                    };

                    if (candidate.IsValid)
                    {
                        map = candidate;
                        return true;
                    }
                }
                catch
                {
                    // Try next row
                }
            }

            return false;
        }

        // -----------------------
        // Meijer Inventory
        // -----------------------
        public readonly struct MeijerHdr
        {
            public int HeaderRow { get; init; }
            public int Upc { get; init; }
            public int Description { get; init; }      // supports "Desctiption" & "Description"
            public int CasePackQty { get; init; }
            public int Sales52Week { get; init; }
            public int StrikePrice { get; init; }

            public bool IsValid =>
                HeaderRow > 0 && Upc > 0 && CasePackQty > 0 && Sales52Week > 0;
        }

        public static bool TryMapMeijerInventory(IXLWorksheet ws, out MeijerHdr hdr)
        {
            const int headerRow = 1;

            try
            {
                var upc = ExcelHeader.Find(ws, headerRow, "UPC");
                var desc = ExcelHeader.Find(ws, headerRow, "Desctiption", "Description"); // typo-friendly
                var caseQty = ExcelHeader.Find(ws, headerRow, "Case Pack Qty", "Case Pack", "CasePackQty");
                var sales52 = ExcelHeader.Find(ws, headerRow, "Sales Qty 52 Week", "Sales Qty 52-Week", "52 Week Sales Qty");

                int strike = 0;
                ExcelHeader.TryFind(ws, headerRow, out strike, "Strike Price", "Strike Cost", "Strike");

                hdr = new MeijerHdr
                {
                    HeaderRow = headerRow,
                    Upc = upc,
                    Description = desc,
                    CasePackQty = caseQty,
                    Sales52Week = sales52,
                    StrikePrice = strike
                };
                return hdr.IsValid;
            }
            catch
            {
                hdr = new MeijerHdr();
                return false;
            }
        }

        // -----------------------
        // Dollar General Inventory (already normalized)
        // -----------------------
        public sealed class DgInventoryHeaderMap
        {
            public int HeaderRow { get; init; }
            public int Upc { get; init; }
            public int Description { get; init; }
            public int CasePack { get; init; }
            public int AvgWklyMvmnt { get; init; }  // renamed
            public int StrikePrice { get; init; }
            public bool IsValid =>
                HeaderRow > 0 && Upc > 0 && Description > 0 && CasePack > 0 && AvgWklyMvmnt > 0;
        }

        public static bool TryMapDgInventory(IXLWorksheet ws, out DgInventoryHeaderMap hdr)
        {
            const int headerRow = 1;
            try
            {
                var upc = ExcelHeader.Find(ws, headerRow, "11 Digit UPC", "11 Digit Upc", "UPC");
                var desc = ExcelHeader.Find(ws, headerRow, "Item\nDescription", "Item Description", "ItemDescription");
                var cspk = ExcelHeader.Find(ws, headerRow, "Cspk", "Case Pack", "CasePack");
                var wkAvg = ExcelHeader.Find(ws, headerRow, "Avg Wkly Mvmnt", "Avg Weekly Movement", "Average Weekly Movement");

                int strike = 0;
                ExcelHeader.TryFind(ws, headerRow, out strike, "Strike Cost", "Strike Price", "Strike");

                hdr = new DgInventoryHeaderMap
                {
                    HeaderRow = headerRow,
                    Upc = upc,
                    Description = desc,
                    CasePack = cspk,
                    AvgWklyMvmnt = wkAvg,
                    StrikePrice = strike
                };
                return hdr.IsValid;
            }
            catch
            {
                hdr = new DgInventoryHeaderMap();
                return false;
            }
        }
    }
}
