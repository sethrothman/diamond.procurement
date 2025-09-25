using System.Globalization;
using ClosedXML.Excel;
using Diamond.Procurement.Data;
using Diamond.Procurement.Domain.Models;
using Diamond.Procurement.Domain.Util;

namespace Diamond.Procurement.App.Processing;

public sealed class CnsInventoryProcessor : BaseExcelProcessor, IFileProcessor, IBuyerScopedProcessor, IUsesSignatureMap<ExcelSignatures.CnsInventoryHeaderMap>
{
    private readonly BuyerInventoryRepository _repo;
    private readonly MasterListRepository _masterListRepo;
    private int _buyerId;

    public FileKind Kind => FileKind.BuyerInventory;

    private ExcelSignatures.CnsInventoryHeaderMap? _map;
    public void SetSignatureMap(ExcelSignatures.CnsInventoryHeaderMap map) => _map = map;

    public CnsInventoryProcessor(BuyerInventoryRepository repo, MasterListRepository masterListRepo)
    {
        _repo = repo;
        _masterListRepo = masterListRepo;
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

        var map = _map ?? (ExcelSignatures.TryMapCnsInventory(ws, out var m) ? m : throw new InvalidOperationException("CNS header row not detected."));

        //if (!ExcelSignatures.TryMapCnsInventory(ws, out var hdr))
        //    throw new InvalidOperationException("CNS Inventory: required headers not found.");

        // Must-have MOS column
        //if (hdr.Mos <= 0)
        //    throw new InvalidOperationException("CNS Inventory: MOS column is required.");

        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "BETHLEHEM 4 GMD",
            "SACRAMENTO GMD",
            "VOORHEESVL GMD",
            "WD JAX GMD"
        };

        var lastRow = ws.LastRowUsed().RowNumber();
        var weeksElapsed = System.Globalization.ISOWeek.GetWeekOfYear(DateTime.Today);
        if (weeksElapsed < 1) weeksElapsed = 1;
        var effective = DateTime.Today;

        // per-UPC accumulation after Pack
        var acc = new Dictionary<string, (long OnHand, long OnPo, double SumAvg13, int CasePack, string? Desc)>(StringComparer.Ordinal);

        for (int r = map.HeaderRow + 1; r <= lastRow; r++)
        {
            ct.ThrowIfCancellationRequested();
            var row = ws.Row(r);
            if (row.IsEmpty()) continue;

            // MOS must be "N"
            var mos = (row.Cell(map.Mos).GetString() ?? string.Empty).Trim();
            if (!mos.Equals("N", StringComparison.OrdinalIgnoreCase)) continue;

            // Location filter (if present)
            if (map.LocationName > 0)
            {
                var loc = (row.Cell(map.LocationName).GetString() ?? string.Empty).Trim();
                if (string.IsNullOrEmpty(loc) || !allowed.Contains(loc))
                    continue;
            }

            // UPC
            var rawUpc = (row.Cell(map.Upc).GetString() ?? string.Empty).Trim();
            if (!UpcNormalizer.TryNormalizeTo10(rawUpc, out var upc, out _))
                continue;

            // Description
            var desc = (row.Cell(map.Description).GetString() ?? string.Empty).Trim();

            // CasePack (for case conversions downstream)
            var masterCase = (int)Math.Round(row.Cell(map.MasterCase).GetDouble());
            if (masterCase <= 0)
            {
                var s = row.Cell(map.MasterCase).GetString();
                if (!int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out masterCase))
                    masterCase = 1;
            }

            // Pack multiplier (default 1)
            var pack = 1;
            if (map.Pack > 0)
            {
                pack = row.Cell(map.Pack).GetIntOrDefault();
                if (pack <= 0) pack = 1;
            }

            // Raw values
            var onHand = row.Cell(map.Boh).GetIntOrDefault();
            var onPo = row.Cell(map.OnOrder).GetIntOrDefault();
            var avg13 = row.Cell(map.Avg13).GetDouble();
            if (avg13 < 0) avg13 = 0;

            // Apply Pack, then accumulate per UPC
            onHand *= pack;
            onPo *= pack;
            avg13 *= pack;

            if (!acc.TryGetValue(upc, out var a))
                a = (0, 0, 0.0, 0, null);

            a.OnHand += onHand;     // SUM BOH (after pack)
            a.OnPo += onPo;       // SUM On Order (after pack)
            a.SumAvg13 += avg13;      // SUM 13wk Avg (after pack)

            if (masterCase > 0)
                a.CasePack = masterCase * pack;

            if (!string.IsNullOrWhiteSpace(desc) &&
                (string.IsNullOrWhiteSpace(a.Desc) || desc.Length > a.Desc!.Length))
                a.Desc = desc;

            acc[upc] = a;
        }

        // Emit one row per UPC
        var rows = new List<BuyerInventoryRow>(acc.Count);
        foreach (var (upc, a) in acc)
        {
            var ytd = (int)Math.Round(a.SumAvg13 * weeksElapsed, MidpointRounding.AwayFromZero);

            rows.Add(new BuyerInventoryRow
            {
                BuyerId = _buyerId,
                Upc = upc,
                CasePack = a.CasePack <= 0 ? 1 : a.CasePack,
                Description = a.Desc ?? string.Empty,
                OnHand = (int)Math.Clamp(a.OnHand, int.MinValue, int.MaxValue),
                OnPo = (int)Math.Clamp(a.OnPo, int.MinValue, int.MaxValue),
                SalesYTD = ytd,
                UnitsSoldLastYear = 0,
                EffectiveDate = DateOnly.FromDateTime(effective)
            });
        }

        await _repo.LoadAsync(rows, ct);
    }
}
