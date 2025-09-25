// Win/Helpers/FileInference.cs

using ClosedXML.Excel;
using Diamond.Procurement.Domain.Util;
using System.IO;
using System.Net;

namespace Diamond.Procurement.Win.Helpers;

public static class FileInference
{
    public enum PartyKind { None = 0, Buyer = 1, Vendor = 2 }

    // Central place to hold your known Buyer IDs; swap to config/DB as needed.
    private static class BuyerIdMap
    {
        public const int LNR = 1;
        public const int CNS = 2;
        public const int MEIJER = 3;
        public const int DG = 4;
        public const int ALL = 5;
    }

    public static string? GetBuyerName(int id) => id switch
    {
        BuyerIdMap.LNR => "L&R",
        BuyerIdMap.CNS => "C&S",
        BuyerIdMap.MEIJER => "Meijer",
        BuyerIdMap.DG => "Dollar General",
        BuyerIdMap.ALL => "All",
        _ => null
    };

    // NEW: rich detection info
    public readonly record struct DetectionInfo(FileKind Kind, PartyKind PartyKind, int? PartyId) // BuyerId for Buyer kinds, VendorId for Vendor kinds
    {
        public object? SignatureMap { get; init; }  // NEW
    }

    // Public friendly-name helper for UI/logging
    public static string GetFriendlyName(FileKind kind) => kind switch
    {
        FileKind.BuyerInventory => "Buyer Inventory",
        FileKind.BuyerForecast => "Buyer Forecast",
        FileKind.VendorForecast => "Vendor Forecast",
        FileKind.MainframeInventory => "Mainframe Inventory",
        FileKind.UpcComp => "Comps Data",
        _ => kind.ToString()
    };

    // (Optional) Friendly description of DetectionInfo if you ever want it
    public static string ToDisplay(DetectionInfo d)
    {
        var name = GetFriendlyName(d.Kind);
        var party = d.PartyKind switch
        {
            PartyKind.Buyer when d.PartyId is int b =>
                $" ({GetBuyerName(b) ?? $"BuyerId {b}"})",
            PartyKind.Buyer => " (Buyer)",
            PartyKind.Vendor when d.PartyId is int v =>
                $" (VendorId {v})",   // replace later if you have vendor map
            PartyKind.Vendor => " (Vendor)",
            _ => string.Empty
        };
        return name + party;
    }


    // NEW: single-pass detection that also returns inferred PartyId when possible
    public static DetectionInfo? TryDetectInfo(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            return null;

        var fileName = Path.GetFileName(path);
        var n = fileName.ToLowerInvariant();
        var ext = Path.GetExtension(fileName).ToLowerInvariant();

        // UPC Comp by exact filename
        if (string.Equals(fileName, "CMPT_RPT_UPC.csv", StringComparison.OrdinalIgnoreCase))
            return new DetectionInfo(FileKind.UpcComp, PartyKind.None, null);

        // 2) CSV mainframe rule (unchanged)
        if (ext == ".csv")
        {
            if (fileName.Contains("upc_app.csv"))
                return new DetectionInfo(FileKind.MainframeInventory, PartyKind.None, null);

            try
            {
                using var reader = File.OpenText(path);
                var header = reader.ReadLine() ?? string.Empty;

                if (header.Contains("upc", StringComparison.OrdinalIgnoreCase) &&
                    header.Contains("on_hand", StringComparison.OrdinalIgnoreCase))
                {
                    return new DetectionInfo(FileKind.MainframeInventory, PartyKind.None, null);
                }

                // UPC Comp distinctive headers
                if (header.Contains("CMPT~VICAL", StringComparison.OrdinalIgnoreCase) ||
                    header.Contains("CMPT~QKALL", StringComparison.OrdinalIgnoreCase))
                {
                    return new DetectionInfo(FileKind.UpcComp, PartyKind.None, null);
                }
            }
            catch { /* ignore */ }

            return null;
        }

        // 3) Excel signatures – SINGLE pass that can set both Kind and PartyId
        if (IsExcel(ext))
            return TryDetectExcelInfo(path);

        return null;
    }

    private static bool IsExcel(string ext) =>
        ext == ".xlsx" || ext == ".xlsm" || ext == ".xltx" || ext == ".xltm";

    private static DetectionInfo? TryDetectExcelInfo(string path)
    {
        try
        {
            using var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var wb = new XLWorkbook(fs);

            // 3a) Buyer Forecast strict (your existing rule)
            if (wb.Worksheets.Count == 1)
            {
                var ws = wb.Worksheets.Worksheet(1)!;
                var sheetName = (ws?.Name ?? string.Empty).Trim();

                if (sheetName.Equals("Vendor Purchase Forecast", StringComparison.OrdinalIgnoreCase))
                {
                    var d1 = GetText(ws, "D1");
                    if (d1.Contains("Vendor Purchase Forecast", StringComparison.OrdinalIgnoreCase) &&
                        d1.Contains("Units", StringComparison.OrdinalIgnoreCase))
                    {
                        return new DetectionInfo(FileKind.BuyerForecast, PartyKind.Buyer, TryInferBuyerIdFromWorkbook(wb));
                    }
                }

                // Buyer Inventory (LNR-style)
                if (sheetName.Equals("New", StringComparison.OrdinalIgnoreCase))
                {
                    var a1 = GetText(ws, "A1");
                    var c1 = GetText(ws, "C1");
                    if (a1.Equals("UPC", StringComparison.OrdinalIgnoreCase) &&
                        c1.Equals("ProductNumber", StringComparison.OrdinalIgnoreCase))
                    {
                        return new DetectionInfo(FileKind.BuyerInventory, PartyKind.Buyer, TryInferBuyerIdFromWorkbook(wb));
                    }
                }

                // CNS Inventory header signature (centralized helper)
                if (ExcelSignatures.TryMapCnsInventory(ws, out var cnsMap))
                    return new DetectionInfo(FileKind.BuyerInventory, PartyKind.Buyer, BuyerIdMap.CNS) { SignatureMap = cnsMap };

                if (ExcelSignatures.TryMapDgInventory(ws, out var dgMap))
                    return new DetectionInfo(FileKind.BuyerInventory, PartyKind.Buyer, BuyerIdMap.DG) { SignatureMap = dgMap };

                // Meijer Inventory header signature
                if (ExcelSignatures.TryMapMeijerInventory(ws, out var mejMap))
                    return new DetectionInfo(FileKind.BuyerInventory, PartyKind.Buyer, BuyerIdMap.MEIJER) { SignatureMap = mejMap };
            }

            // 3b) Vendor Forecast dynamic header row (unchanged kind, no party inference here yet)
            {
                var ws = wb.Worksheets.Worksheet(1);
                var headerRow = App.Util.ForecastHeaderHelpers.FindVendorHeaderRow(ws);
                if (headerRow > 0)
                    return new DetectionInfo(FileKind.VendorForecast, PartyKind.Vendor, TryInferVendorIdFromWorkbook(wb));
            }

            // 3c) Fallbacks across sheets
            foreach (var ws in wb.Worksheets)
            {
                if (ws.Name.Equals("Vendor Purchase Forecast", StringComparison.OrdinalIgnoreCase))
                    return new DetectionInfo(FileKind.BuyerForecast, PartyKind.Buyer, TryInferBuyerIdFromWorkbook(wb));

                if (ExcelSignatures.TryMapCnsInventory(ws, out _))
                    return new DetectionInfo(FileKind.BuyerInventory, PartyKind.Buyer, BuyerIdMap.CNS);

                if (ExcelSignatures.TryMapMeijerInventory(ws, out _))
                    return new DetectionInfo(FileKind.BuyerInventory, PartyKind.Buyer, BuyerIdMap.MEIJER);
            }

            // 3d) Buyer Inventory fallback (LNR-style row-1 headers)
            {
                var ws = wb.Worksheets.Worksheet(1);
                var a1 = GetText(ws, "A1");
                var c1 = GetText(ws, "C1");
                if (a1.Equals("UPC", StringComparison.OrdinalIgnoreCase) &&
                    c1.Equals("ProductNumber", StringComparison.OrdinalIgnoreCase))
                {
                    return new DetectionInfo(FileKind.BuyerInventory, PartyKind.Buyer, TryInferBuyerIdFromWorkbook(wb));
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    private static int? TryInferBuyerIdFromWorkbook(XLWorkbook wb)
    {
        // Header-based inference — we already scanned sheets; if any sheet maps CNS, call it CNS.
        foreach (var ws in wb.Worksheets)
            if (ExcelSignatures.TryMapCnsInventory(ws, out _))
                return BuyerIdMap.CNS;

        // Add LNR-specific signature here if you define one (optional)
        return BuyerIdMap.LNR;
    }

    private static int? TryInferVendorIdFromWorkbook(XLWorkbook wb)
    {
        // Add vendor workbook heuristics as needed
        return null;
    }

    private static string GetText(IXLWorksheet ws, string address)
    {
        try
        {
            var cell = ws.Cell(address);
            var s = cell.GetFormattedString();
            if (string.IsNullOrWhiteSpace(s))
                s = cell.GetString();
            return (s ?? string.Empty).Trim();
        }
        catch { return string.Empty; }
    }
}
