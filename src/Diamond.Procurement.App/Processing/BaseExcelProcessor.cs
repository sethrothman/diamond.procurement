using ClosedXML.Excel;
using System.Globalization;

namespace Diamond.Procurement.App.Processing;

public abstract class BaseExcelProcessor
{
    protected static int FindColumn(IXLWorksheet ws, int headerRow, params string[] candidates)
    {
        var row = ws.Row(headerRow);
        foreach (var cell in row.CellsUsed())
        {
            var text = (cell.GetString() ?? string.Empty).Trim();
            foreach (var c in candidates)
                if (text.Equals(c, StringComparison.OrdinalIgnoreCase))
                    return cell.Address.ColumnNumber;
        }
        throw new InvalidOperationException($"Could not find column matching: {string.Join(", ", candidates)} on header row {headerRow}");
    }

    protected static DateOnly TodayDateOnly() => DateOnly.FromDateTime(DateTime.UtcNow.Date);

    protected static bool TryParseMonthHeader(string s, out DateTime date)
    {
        s = (s ?? string.Empty).Trim();
        if (DateTime.TryParse(s, out date)) return true;

        var fmts = new[] { "MMM yyyy", "MMM-yy", "yyyy-MM", "MM/yyyy", "MM-yy" };
        foreach (var f in fmts)
            if (DateTime.TryParseExact(s, f, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                return true;

        if (double.TryParse(s, out var serial))
        {
            try { date = DateTime.FromOADate(serial); return true; }
            catch { /* ignore */ }
        }
        date = default;
        return false;
    }
}