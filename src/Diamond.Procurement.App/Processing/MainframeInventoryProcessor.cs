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

        static int ParseMainframeInt(string? raw)
        {
            // Handles: "123", "-123", "123-", "1,234", "1,234-"
            var s = (raw ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(s)) return 0;

            s = s.Replace(",", "");
            bool trailingMinus = s.EndsWith('-');
            if (trailingMinus) s = s[..^1];

            if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var val))
                return trailingMinus ? -val : val;

            return int.TryParse((raw ?? "").Trim(), out var v) ? v : 0;
        }

        static decimal? ParseMoney(string? raw)
        {
            // Accept: "$1,234.56", "1,234.56", "(1,234.56)", "1234", "1234-"
            var s = (raw ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(s)) return null;

            var neg = false;
            if (s.EndsWith("-", StringComparison.Ordinal))
            {
                neg = true; s = s[..^1];
            }
            if (s.StartsWith("(") && s.EndsWith(")"))
            {
                neg = true; s = s.Trim('(', ')');
            }

            s = s.Replace("$", "").Replace(",", "");
            if (decimal.TryParse(s, NumberStyles.Number | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var val))
                return neg ? -val : val;

            return null;
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

            var qty = ParseMainframeInt(csv.GetField(hQty));
            var onpo = ParseMainframeInt(csv.GetField(hOnPo));
            var ovstk = ParseMainframeInt(csv.GetField(hOvstk));
            var list = ParseMoney(csv.GetField(hList)); // NEW
            var hi = ParseMainframeInt(csv.GetField(hHi));
            var ti = ParseMainframeInt(csv.GetField(hTi));

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
