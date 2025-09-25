// File: src/Diamond.Procurement.Domain/Util/ExcelHeader.cs
using System;
using System.Linq;
using System.Text;
using ClosedXML.Excel;

namespace Diamond.Procurement.Domain.Util
{
    /// <summary>
    /// Normalized header lookup for Excel sheets.
    /// Canonicalizes header text (remove non-alnum, uppercase) and matches on any of the provided aliases.
    /// </summary>
    public static class ExcelHeader
    {
        public static int Find(IXLWorksheet ws, int headerRow, params string[] aliases)
        {
            if (ws == null) throw new ArgumentNullException(nameof(ws));
            if (aliases == null || aliases.Length == 0) throw new ArgumentException("At least one alias required.", nameof(aliases));

            var targets = aliases.Select(Canon).ToArray();

            var lastCol = ws.Row(headerRow).LastCellUsed()?.Address.ColumnNumber
                          ?? ws.LastColumnUsed()?.ColumnNumber() ?? 0;
            if (lastCol == 0)
                throw new InvalidOperationException($"Row {headerRow} has no used header cells.");

            for (int c = 1; c <= lastCol; c++)
            {
                var raw = ws.Cell(headerRow, c).GetString() ?? string.Empty;
                if (targets.Contains(Canon(raw)))
                    return c;
            }

            throw new InvalidOperationException($"Could not find any of [{string.Join(", ", aliases)}] on row {headerRow}.");
        }

        public static bool TryFind(IXLWorksheet ws, int headerRow, out int col, params string[] aliases)
        {
            try
            {
                col = Find(ws, headerRow, aliases);
                return true;
            }
            catch
            {
                col = 0;
                return false;
            }
        }

        public static string Canon(string s)
        {
            if (s == null) return string.Empty;
            var sb = new StringBuilder(s.Length);
            foreach (var ch in s)
            {
                if (char.IsLetterOrDigit(ch))
                    sb.Append(char.ToUpperInvariant(ch));
            }
            return sb.ToString();
        }
    }
}
