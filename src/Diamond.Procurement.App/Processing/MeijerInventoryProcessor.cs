using ClosedXML.Excel;
using Diamond.Procurement.Data;
using Diamond.Procurement.Domain.Models;
using Diamond.Procurement.Domain.Util;
using System.Globalization;

namespace Diamond.Procurement.App.Processing;

public sealed class MeijerInventoryProcessor : BaseExcelProcessor, IFileProcessor, IBuyerScopedProcessor, IUsesSignatureMap<ExcelSignatures.MeijerHdr>
{
    private readonly BuyerInventoryRepository _repo;
    private int _buyerId;

    public FileKind Kind => FileKind.BuyerInventory;

    private ExcelSignatures.MeijerHdr? _map;
    public void SetSignatureMap(ExcelSignatures.MeijerHdr map) => _map = map;

    public MeijerInventoryProcessor(BuyerInventoryRepository repo)
    {
        _repo = repo;
    }

    public Task SetBuyer(int buyerId, CancellationToken ct)
    {
        _buyerId = buyerId;
        return Task.CompletedTask;
    }

    public async Task ProcessAsync(string path, CancellationToken ct)
    {
        using var wb = new XLWorkbook(path);
        var ws = wb.Worksheets.First();

        var map = _map ?? (ExcelSignatures.TryMapMeijerInventory(ws, out var m) ? m : throw new InvalidOperationException("Meijer header row not detected."));

        //// Map headers (row 1 per sample)
        //var map = MapHeaders(ws);
        //if (map.Upc <= 0 || map.CasePackQty <= 0 || map.Sales52Week <= 0)
        //    throw new InvalidOperationException("Meijer Inventory: required headers not found (UPC, Case Pack Qty, Sales Qty 52 Week).");

        // Effective date from filename (e.g. "... 8-15-25 ...")
        var effective = FileNameDateParser.ExtractDateFromFileName(Path.GetFileName(path));
        var weeksElapsed = ISOWeek.GetWeekOfYear(effective);
        if (weeksElapsed < 1) weeksElapsed = 1;

        var lastRow = ws.LastRowUsed().RowNumber();
        var rows = new List<BuyerInventoryRow>();

        for (int r = map.HeaderRow + 1; r <= lastRow; r++)
        {
            ct.ThrowIfCancellationRequested();
            var row = ws.Row(r);
            if (row.IsEmpty()) continue;

            // UPC (normalize to 10 where possible; if you have UpcNormalizer, use it)
            var rawUpc = (row.Cell(map.Upc).GetString() ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(rawUpc)) continue;

            string upc = rawUpc;
            // If you want strict 10-digit normalization as elsewhere:
            if (!UpcNormalizer.TryNormalizeTo10(rawUpc, out upc, out _)) continue;

            // Description (support both "Desctiption" and "Description")
            var desc = map.Description > 0 ? (row.Cell(map.Description).GetString() ?? "").Trim() : "";

            // CasePack
            var casePack = SafeToInt(row.Cell(map.CasePackQty));
            if (casePack <= 0) casePack = 1;

            // Movement mapping:
            // Original 52-week annualized value → UnitsSoldLastYear; compute SalesYTD from it.
            var annual = SafeToInt(row.Cell(map.Sales52Week)); // integer “annualized” count
            var weekly = annual / 52.0;
            var ytd = (int)Math.Round(weekly * weeksElapsed, MidpointRounding.AwayFromZero);

            // Strike Price (nullable money)
            decimal? strike = null;
            if (map.StrikePrice > 0)
            {
                var c = row.Cell(map.StrikePrice);
                if (c.DataType == XLDataType.Number) strike = (decimal)c.GetDouble();
                else
                {
                    var s = (c.GetString() ?? "").Trim();
                    if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                        strike = d;
                }
            }

            rows.Add(new BuyerInventoryRow
            {
                BuyerId = _buyerId,
                Upc = upc,
                Description = desc,
                CasePack = casePack,
                OnHand = 0,                // not present in Meijer sample
                OnPo = 0,                  // not present in Meijer sample
                SalesYTD = ytd,
                UnitsSoldLastYear = annual,
                EffectiveDate = DateOnly.FromDateTime(effective),
                StrikePrice = strike
            });
        }

        await _repo.LoadAsync(rows, ct);
    }

    private static int SafeToInt(IXLCell cell)
    {
        if (cell.DataType == XLDataType.Number) return (int)Math.Round(cell.GetDouble());

        var s = (cell.GetString() ?? string.Empty).Trim();
        if (int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var n)) return n;
        if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d)) return (int)Math.Round(d);
        return 0;
    }
}
