using ClosedXML.Excel;
using Diamond.Procurement.App.Util;
using Diamond.Procurement.Domain.Models;
using Diamond.Procurement.Win.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Diamond.Procurement.Win.Helpers
{
    public static class OrderPlacementHelper
    {
        /// <summary>
        /// Locates UPC + Quantity columns in an .xlsx vendor order sheet at <paramref name="workbookPath"/>,
        /// and writes QtyInCases for all rows where QtyInCases > 0. Saves as:
        ///   OriginalFileName - listType (orderMonth).xlsx
        /// Returns the number of rows updated.
        /// NOTE: This method is UI-free and intended to be called from code that handles dialogs/overlays.
        /// </summary>
        public static async Task<VendorOrderResult> PopulateVendorOrderSheetAsync(
            IEnumerable<VendorOrderRowVM> sourceRows,
            string workbookPath,
            string listType,
            string orderMonth)
        {
            var orderLines = sourceRows
                .Where(r => r != null && r.QtyInCases > 0 && !string.IsNullOrWhiteSpace(r.Upc))
                .Select(r => new { Upc = r.Upc!.Trim(), Qty = r.QtyInCases })
                .ToList();

            if (orderLines.Count == 0)
                return new VendorOrderResult(0, workbookPath);

            if (!string.Equals(Path.GetExtension(workbookPath), ".xlsx", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only .xlsx files are supported.");

            return await Task.Run(() =>
            {
                int updated = 0;

                using var wb = new XLWorkbook(workbookPath);
                var ws = wb.Worksheets.FirstOrDefault()
                         ?? throw new InvalidOperationException("The workbook does not contain a worksheet.");

                var used = ws.RangeUsed()
                          ?? throw new InvalidOperationException("The worksheet is empty.");

                var headerRowNum = ForecastHeaderHelpers.FindVendorHeaderRow(ws);
                var headerRange = ws.Range(headerRowNum, used.FirstColumn().ColumnNumber(),
                                           headerRowNum, used.LastColumn().ColumnNumber());

                int colUpc = -1, colQty = -1;
                foreach (var cell in headerRange.Cells())
                {
                    var name = (cell.GetString() ?? string.Empty).Trim();
                    if (name.Equals("ITEM UPC", StringComparison.OrdinalIgnoreCase)) colUpc = cell.Address.ColumnNumber;
                    if (name.Equals("Order Cases", StringComparison.OrdinalIgnoreCase)) colQty = cell.Address.ColumnNumber;
                }

                if (colUpc < 0 || colQty < 0)
                    throw new InvalidOperationException("Could not locate 'UPC' and/or 'Order Quantity' column headers.");

                // Build a lookup map
                var lastDataRow = used.LastRow().RowNumber();
                var upcToRow = new Dictionary<string, int>(StringComparer.Ordinal);
                for (int r = headerRowNum + 1; r <= lastDataRow; r++)
                {
                    var raw = ws.Cell(r, colUpc).GetString() ?? string.Empty;
                    var norm = ForecastHeaderHelpers.CleanLibertyUpc(raw);
                    if (!string.IsNullOrEmpty(norm) && !upcToRow.ContainsKey(norm))
                        upcToRow[norm] = r;
                }

                foreach (var line in orderLines)
                {
                    var normUpc = ForecastHeaderHelpers.CleanLibertyUpc(line.Upc);
                    if (!upcToRow.TryGetValue(normUpc, out var rowNum))
                        continue;

                    ws.Cell(rowNum, colQty).Value = line.Qty;
                    updated++;
                }

                // SaveAs new file
                var dir = Path.GetDirectoryName(workbookPath)!;
                var filenameWithoutExt = Path.GetFileNameWithoutExtension(workbookPath);
                var ext = Path.GetExtension(workbookPath);
                var newFileName = $"{filenameWithoutExt} - {listType} ({orderMonth}){ext}";
                var newPath = Path.Combine(dir, newFileName);

                wb.SaveAs(newPath);

                return new VendorOrderResult(updated, newPath);
            });
        }
    }
}
