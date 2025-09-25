using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Diamond.Procurement.Data;
using Diamond.Procurement.Domain.Models;
using Diamond.Procurement.Domain.Util;

namespace Diamond.Procurement.App.Processing;

public sealed class MainframeInventoryProcessor : IFileProcessor
{
    private readonly MainframeInventoryRepository _repo;

    public FileKind Kind => FileKind.MainframeInventory;

    public MainframeInventoryProcessor(MainframeInventoryRepository repo)
    {
        _repo = repo;
    }

    public async Task ProcessAsync(string path, CancellationToken ct)
    {
        var effective = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var rows = new List<MainframeInventoryRow>(50000);

        using var sr = new StreamReader(path);
        // Header starts on row 2; skip row 1 (title/metadata line)
        _ = await sr.ReadLineAsync(ct);

        var cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            BadDataFound = null,
            MissingFieldFound = null,
            HeaderValidated = null,
            TrimOptions = TrimOptions.Trim,
            DetectDelimiter = true
        };

        using var csv = new CsvReader(sr, cfg);
        await csv.ReadAsync();
        csv.ReadHeader();
        var headers = csv.HeaderRecord?.ToArray() ?? Array.Empty<string>();

        string FindHeader(params string[] candidates)
        {
            foreach (var h in headers)
            {
                var hs = (h ?? string.Empty).Trim();
                foreach (var c in candidates)
                    if (hs.Contains(c, StringComparison.OrdinalIgnoreCase))
                        return h;
            }
            throw new InvalidOperationException($"Could not find header: {string.Join(", ", candidates)}");
        }

        var hUpc = FindHeader("UPC");
        var hCspk = FindHeader("CSPK");
        var hQty = FindHeader("QTY~AVL");
        var hOnPo = FindHeader("ON~PO");
        var hOvstk = FindHeader("OVERSTOCK", "OVSTCK");
        var hDesc = FindHeader("DESC");
        var hList = FindHeader("LIST");
        var hHi = FindHeader("HI");
        var hTi = FindHeader("TI");

        while (await csv.ReadAsync())
        {
            var rawUpc = (csv.GetField(hUpc) ?? string.Empty).Trim();
            if (!UpcNormalizer.TryNormalizeTo10(rawUpc, out var upc, out _)) continue;

            var desc = (csv.GetField(hDesc) ?? string.Empty).Trim();

            int? casePack = null;
            var cspkStr = csv.GetField(hCspk);
            if (int.TryParse((cspkStr ?? string.Empty).Trim(), out var cp) && cp > 0)
                casePack = cp;

            var qty = NumericParsers.ParseSignedIntOrDefault(csv.GetField(hQty));
            var onpo = NumericParsers.ParseSignedIntOrDefault(csv.GetField(hOnPo));
            var ovstk = NumericParsers.ParseSignedIntOrDefault(csv.GetField(hOvstk));
            var list = NumericParsers.ParseMoney(csv.GetField(hList));
            var hi = NumericParsers.ParseSignedIntOrDefault(csv.GetField(hHi));
            var ti = NumericParsers.ParseSignedIntOrDefault(csv.GetField(hTi));

            rows.Add(new MainframeInventoryRow
            {
                Upc = upc,
                Description = desc,
                CasePack = casePack,
                QtyAvailable = qty,
                QtyOnPo = onpo,
                QtyOverstock = ovstk,
                ListPrice = list,               
                EffectiveDate = effective,
                HI = hi,
                TI = ti,
            });
        }

        await _repo.LoadAsync(rows, ct);
    }
}
