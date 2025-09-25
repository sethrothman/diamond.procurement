using ClosedXML.Excel;
using Diamond.Procurement.Data;
using Diamond.Procurement.Domain.Models;
using Diamond.Procurement.Domain.Util;

namespace Diamond.Procurement.App.Processing;

public sealed class LnrForecastProcessor : BaseExcelProcessor, IFileProcessor
{
    private readonly Data.BuyerForecastRepository _repo;
    private readonly MasterListRepository _masterListRepo;
    private int _buyerId;

    public FileKind Kind => FileKind.BuyerForecast;

    public LnrForecastProcessor(BuyerForecastRepository repo, MasterListRepository masterListRepo, int buyerId = 1)
    {
        _repo = repo;
        _masterListRepo = masterListRepo;
        _buyerId = buyerId;
    }

    public async Task SetBuyer(int masterListId, CancellationToken ct) 
    {
        var row = await _masterListRepo.GetByIdAsync(masterListId, ct);
        _buyerId = row.BuyerId;
    }

    public async Task ProcessAsync(string path, CancellationToken ct)
    {
        if (_buyerId <= 0) throw new InvalidOperationException("BuyerId not set.");

        using var wb = new XLWorkbook(path);
        var ws = wb.Worksheets.First();
        const int headerRow = 5;

        int colUpc = FindColumn(ws, headerRow, "UPC");
        int colDesc = FindColumn(ws, headerRow, "Product Description");

        // find first month column on the header row
        var headerCells = ws.Row(headerRow).CellsUsed().ToList();
        var firstMonthCell = headerCells.FirstOrDefault(c => TryParseMonthHeader(c.GetString(), out _))
            ?? throw new InvalidOperationException("Could not locate first forecast month column.");
        int startCol = firstMonthCell.Address.ColumnNumber;

        var rows = new List<BuyerForecastRow>(24000);

        var lastRow = ws.LastRowUsed()?.RowNumber() ?? headerRow;
        for (int rnum = headerRow + 1; rnum <= lastRow; rnum++)
        {
            var r = ws.Row(rnum);

            var rawUpc = r.Cell(colUpc).GetString();
            if (!UpcNormalizer.TryNormalizeTo10(rawUpc, out var upc, out _)) continue;

            var desc = r.Cell(colDesc).GetString().Trim();

            for (int i = 0; i < 12; i++)
            {
                var hdr = ws.Cell(headerRow, startCol + i).GetString();
                if (!TryParseMonthHeader(hdr, out var dt)) continue;

                var qty = (int)r.Cell(startCol + i).GetDoubleOrDefault();
                if (qty <= 0) continue;

                rows.Add(new BuyerForecastRow
                {
                    BuyerId = _buyerId,
                    Description = desc,
                    Upc = upc,
                    QtyInUnits = qty,                          // leaving your model name as-is
                    ForecastDate = DateOnly.FromDateTime(dt),    // matches your current model
                });
            }
        }

        await _repo.LoadAsync(rows, ct);
    }
}