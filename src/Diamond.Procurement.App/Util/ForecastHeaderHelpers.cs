using System;
using System.Linq;
using ClosedXML.Excel;

namespace Diamond.Procurement.App.Util
{
    public static class ForecastHeaderHelpers
    {
        /// <summary>
        /// Scans down column A starting at row 1 to find the header row that contains "Category".
        /// Falls back to 12 if not found.
        /// </summary>
        public static int FindVendorHeaderRow(IXLWorksheet ws, string anchor = "Category", int startRow = 1, int maxScanRows = 300)
        {
            var anchorLower = (anchor ?? "Category").Trim().ToLowerInvariant();
            var last = Math.Min(ws.LastRowUsed()?.RowNumber() ?? 1, maxScanRows);
            for (int r = Math.Max(1, startRow); r <= last; r++)
            {
                var a = (ws.Cell(r, 1).GetFormattedString() ?? string.Empty).Trim();
                if (a.Equals(anchorLower, StringComparison.OrdinalIgnoreCase) ||
                    a.Equals("Category", StringComparison.OrdinalIgnoreCase))
                {
                    return r;
                }
            }

            // Safety fallback to prior convention:
            return 12;
        }

        /// <summary>
        /// Liberty now provides UPC as "xxxxx-xxxxxEA". Remove hyphen and trailing "EA",
        /// and return only digits for downstream normalization.
        /// </summary>
        public static string CleanLibertyUpc(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;

            var s = raw.Trim();

            // Remove trailing "EA" (case-insensitive)
            if (s.EndsWith("EA", StringComparison.OrdinalIgnoreCase))
                s = s[..^2];

            // Remove hyphens
            s = s.Replace("-", "", StringComparison.Ordinal);

            // Keep digits only (belt & suspenders)
            s = new string(s.Where(char.IsDigit).ToArray());

            return s;
        }
    }
}
