using ClosedXML.Excel;
using Diamond.Procurement.Data;
using Diamond.Procurement.Domain.Util;
using Diamond.Procurement.Domain.Models;
using System.Globalization;

namespace Diamond.Procurement.App.Processing;

public sealed class DgInventoryProcessor
    : BaseExcelProcessor,
      IFileProcessor,
      IBuyerScopedProcessor,
      IUsesSignatureMap<ExcelSignatures.DgInventoryHeaderMap>   // NEW
{
    private readonly BuyerInventoryRepository _repo;
    private int _buyerId;
    private ExcelSignatures.DgInventoryHeaderMap? _map;         // NEW

    public FileKind Kind => FileKind.BuyerInventory;

    public DgInventoryProcessor(BuyerInventoryRepository repo) => _repo = repo;

    public Task SetBuyer(int buyerId, CancellationToken ct) { _buyerId = buyerId; return Task.CompletedTask; }
    public void SetSignatureMap(ExcelSignatures.DgInventoryHeaderMap map) => _map = map; // NEW

    public async Task ProcessAsync(string path, CancellationToken ct)
    {
        using var wb = new XLWorkbook(path);
        var ws = wb.Worksheets.First();

        // Use provided map if present; otherwise detect here (no duplicate mapping code).
        var map = _map ?? (ExcelSignatures.TryMapDgInventory(ws, out var m)
            ? m
            : throw new InvalidOperationException("DG header row not detected."));

        var effectiveDt = FileNameDateParser.ExtractDateFromFileName(Path.GetFileName(path));
        var weekNum = Math.Clamp(System.Globalization.ISOWeek.GetWeekOfYear(effectiveDt), 1, 52);

        var lastRow = ws.LastRowUsed().RowNumber();
        var outRows = new List<BuyerInventoryRow>();

        for (int r = map.HeaderRow + 1; r <= lastRow; r++)
        {
            ct.ThrowIfCancellationRequested();
            var row = ws.Row(r);
            if (row.IsEmpty()) continue;

            var rawUpc = (row.Cell(map.Upc).GetString() ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(rawUpc)) continue;

            var cleaned = rawUpc.Replace("-", "").Replace(" ", "").ToUpperInvariant();
            if (cleaned.EndsWith("EA")) cleaned = cleaned[..^2];
            if (cleaned.Length == 11 && cleaned.All(char.IsDigit))
                cleaned = cleaned[1..]; // DG 11-digit -> drop leftmost

            if (!UpcNormalizer.TryNormalizeTo10(cleaned, out var upc10, out _))
                continue;

            var desc = (row.Cell(map.Description).GetString() ?? string.Empty).Trim();

            // CasePack still mapped, but not used for weekly eaches calc.
            var casePack = row.Cell(map.CasePack).GetIntOrDefault();
            if (casePack <= 0) casePack = 1;

            // Avg Wkly Mvmnt is already IN EACHES
            var weeklyEaches = SafeToDecimal(row.Cell(map.AvgWklyMvmnt));
            var salesYtdUnits = (int)Math.Round(weeklyEaches * weekNum, MidpointRounding.AwayFromZero);
            var lastYearUnits = (int)Math.Round(weeklyEaches * 52m, MidpointRounding.AwayFromZero);

            decimal? strike = null;
            if (map.StrikePrice > 0)
            {
                var c = row.Cell(map.StrikePrice);
                if (c.DataType == XLDataType.Number)
                    strike = (decimal)c.GetDouble();
                else if (decimal.TryParse(c.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
                    strike = d;

                if (strike.HasValue)
                    strike = Math.Round(strike.Value * casePack, 2);
            }

            outRows.Add(new BuyerInventoryRow
            {
                BuyerId = _buyerId,
                Upc = upc10,
                Description = desc,
                CasePack = casePack,
                OnHand = 0,
                OnPo = 0,
                SalesYTD = salesYtdUnits,
                UnitsSoldLastYear = lastYearUnits,
                EffectiveDate = DateOnly.FromDateTime(effectiveDt),
                StrikePrice = strike
            });
        }

        await _repo.LoadAsync(outRows, ct);
    }

    private static decimal SafeToDecimal(IXLCell cell)
        => cell.DataType == XLDataType.Number ? (decimal)cell.GetDouble()
           : (decimal.TryParse((cell.GetString() ?? "").Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var m) ? m
           : (double.TryParse((cell.GetString() ?? "").Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? (decimal)d : 0m));
}
