using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Diamond.Procurement.Data;
using Diamond.Procurement.Domain.Models;
using Diamond.Procurement.Domain.Util;

namespace Diamond.Procurement.App.Processing
{
    public sealed class UpcCompProcessor : IFileProcessor
    {
        private readonly UpcCompRepository _repo;

        public FileKind Kind => FileKind.UpcComp;

        public UpcCompProcessor(UpcCompRepository repo)
        {
            _repo = repo;
        }

        public async Task ProcessAsync(string path, CancellationToken ct = default)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(path);

            var cfg = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                BadDataFound = null,
                MissingFieldFound = null,
                TrimOptions = TrimOptions.Trim
            };

            var rows = new List<UpcCompRow>(capacity: 4096);

            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, cfg);
            await csv.ReadAsync();
            csv.ReadHeader();

            var headers = csv.HeaderRecord?.ToArray() ?? Array.Empty<string>();

            // Required columns (note: “ CMPT~VICAL” can have a leading space)
            int idxUpc = IndexOfHeader(headers, "UPC");
            int idxVical = IndexOfHeader(headers, "CMPT~Vic", allowLeadingSpace: true);
            int idxVicalQty = idxVical + 1; // “CMPT~QTY” immediately to the right
            int idxQkall = IndexOfHeader(headers, "CMPT~Qk");
            int idxQkallQty = idxQkall + 1; // “CMPT~QTY” immediately to the right

            while (await csv.ReadAsync())
            {
                var rawUpc = csv.GetField(idxUpc) ?? string.Empty;
                if (!UpcNormalizer.TryNormalizeTo10(rawUpc, out var upc, out _))
                    continue;

                decimal? priceVI = NumericParsers.ParseMoney(csv.GetField(idxVical));
                int? qtyVI = NumericParsers.ParseOptionalSignedInt(csv.GetField(idxVicalQty));
                decimal? priceQK = NumericParsers.ParseMoney(csv.GetField(idxQkall));
                int? qtyQK = NumericParsers.ParseOptionalSignedInt(csv.GetField(idxQkallQty));

                // Skip rows that carry no useful signal
                if (priceVI is null && qtyVI is null && priceQK is null && qtyQK is null)
                    continue;

                rows.Add(new UpcCompRow
                {
                    Upc = upc,
                    PriceVictory = priceVI,
                    QtyVictory = qtyVI,
                    PriceQualityKing = priceQK,
                    QtyQualityKing = qtyQK
                });
            }

            await _repo.LoadAsync(rows, ct);
        }

        private static int IndexOfHeader(string[] headers, string target, bool allowLeadingSpace = false)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                var h = headers[i] ?? string.Empty;
                var hs = allowLeadingSpace ? h.TrimStart() : h.Trim();
                if (hs.Equals(target, StringComparison.OrdinalIgnoreCase))
                    return i;
            }
            throw new InvalidOperationException($"Could not find column header '{target}'.");
        }

    }
}
