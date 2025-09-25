// App\Processing\LibertyShipmentItemsProcessor.cs
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using Diamond.Procurement.App.Util;             // CleanLibertyUpc

namespace Diamond.Procurement.App.Processing;

public sealed record ShipmentItem(string Upc, int Quantity);
public sealed record ShipmentParseResult(string? InvoiceNum, int? TruckNum, List<ShipmentItem> Items);

public interface IShipmentItemsProcessor
{
    bool CanHandle(string path);
    ShipmentParseResult Parse(string path);
}

public sealed class LibertyShipmentItemsProcessor : BaseExcelProcessor, IShipmentItemsProcessor
{
    private static readonly Regex FileRx =
        new(@"(?<inv>LB\d+)\b.*?\bT(?<truck>\d+)\b", RegexOptions.IgnoreCase);

    public bool CanHandle(string path)
        => Path.GetFileName(path).StartsWith("LB", StringComparison.OrdinalIgnoreCase)
        && (path.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".xls", StringComparison.OrdinalIgnoreCase));

    public ShipmentParseResult Parse(string path)
    {
        // 1) File name → Invoice, Truck
        string? invoice = null; int? truck = null;
        var name = Path.GetFileNameWithoutExtension(path);
        var m = FileRx.Match(name);
        if (m.Success)
        {
            invoice = m.Groups["inv"].Value;
            if (int.TryParse(m.Groups["truck"].Value, out var t)) truck = t;
        }

        // 2) Excel → locate headers using BaseExcelProcessor.FindColumn (no RangeUsed)
        using var wb = new XLWorkbook(path);
        var ws = wb.Worksheets.First();

        var headerRow = FindHeaderRow(ws, maxScan: 30) ?? 1;

        int colUpc = FindColumn(ws, headerRow, "Item Code");
        int colQty = FindColumn(ws, headerRow, "Qty Shipped");

        // 3) Rows → items
        var lastRow = ws.LastRowUsed()?.RowNumber() ?? headerRow;
        var items = new List<ShipmentItem>(512);

        for (int r = headerRow + 1; r <= lastRow; r++)
        {
            var upcRaw = ws.Cell(r, colUpc).GetString()?.Trim();
            if (string.IsNullOrWhiteSpace(upcRaw)) continue;

            var cleaned = ForecastHeaderHelpers.CleanLibertyUpc(upcRaw); // digits-only, strip - and EA
            if (string.IsNullOrEmpty(cleaned)) continue;

            int qty;
            var qtyCell = ws.Cell(r, colQty);
            var qtyStr = qtyCell.GetString()?.Trim();
            if (!int.TryParse(qtyStr, out qty))
                qty = (int)Math.Round(qtyCell.GetDouble(), MidpointRounding.AwayFromZero);

            if (qty > 0)
                items.Add(new ShipmentItem(cleaned, qty));
        }

        return new ShipmentParseResult(invoice, truck, items);
    }

    /// <summary>
    /// Scan the first N rows to find a header row that contains both UPC and Quantity headers.
    /// Uses CellsUsed() (no RangeUsed).
    /// </summary>
    private static int? FindHeaderRow(IXLWorksheet ws, int maxScan = 30)
    {
        var last = Math.Min(ws.LastRowUsed()?.RowNumber() ?? 1, maxScan);
        for (int r = 1; r <= last; r++)
        {
            var cells = ws.Row(r).CellsUsed().Select(c => (Text: (c.GetString() ?? "").Trim(), Col: c.Address.ColumnNumber)).ToList();
            if (cells.Count == 0) continue;

            bool hasUpc = cells.Any(c => EqualsIgnoreCase(c.Text, "UPC") || EqualsIgnoreCase(c.Text, "ITEM UPC"));
            bool hasQty = cells.Any(c => EqualsIgnoreCase(c.Text, "QTY") || EqualsIgnoreCase(c.Text, "QUANTITY")
                                      || EqualsIgnoreCase(c.Text, "QTY EA") || EqualsIgnoreCase(c.Text, "QTY EACH")
                                      || EqualsIgnoreCase(c.Text, "QTY IN EACHES"));

            if (hasUpc && hasQty) return r;
        }
        return null;

        static bool EqualsIgnoreCase(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
    }
}
