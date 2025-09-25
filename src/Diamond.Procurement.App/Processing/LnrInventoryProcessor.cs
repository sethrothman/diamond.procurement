using ClosedXML.Excel;
using Diamond.Procurement.Data;
using Diamond.Procurement.Domain.Models;
using Diamond.Procurement.Domain.Util;

namespace Diamond.Procurement.App.Processing;

public sealed class LnrInventoryProcessor : BaseExcelProcessor, IFileProcessor, IBuyerScopedProcessor
{
    private readonly BuyerInventoryRepository _repo;
    private readonly MasterListRepository _masterListRepo;
    private int _buyerId;

    public FileKind Kind => FileKind.BuyerInventory;

    public LnrInventoryProcessor(BuyerInventoryRepository repo, MasterListRepository masterListRepo, int buyerId = 1)
    {
        _repo = repo;
        _masterListRepo = masterListRepo;

        _buyerId = buyerId;
    }

    public async Task SetBuyer(int masterListId, CancellationToken ct) 
    {
        var row = await _masterListRepo.GetByIdAsync(masterListId, ct);
        _buyerId = row!.BuyerId;  
    }

    public async Task ProcessAsync(string path, CancellationToken ct)
    {
        if (_buyerId <= 0) throw new InvalidOperationException("BuyerId not set.");

        using var wb = new XLWorkbook(path);
        var ws = wb.Worksheets.First();

        const int headerRow = 1;

        // more forgiving header detection (supports old/new names)
        int colUpc = FindColumn(ws, headerRow, "UPC");
        int colYTD = FindColumn(ws, headerRow, "UnitsSoldYTD");                 
        int colPrior = FindColumn(ws, headerRow, "UnitsSoldPriorYear"); 
        int cA8 = FindColumn(ws, headerRow, "AvailQtyWhse8");
        int cP8 = FindColumn(ws, headerRow, "OnPOWhse8");
        int cA4 = FindColumn(ws, headerRow, "AvailQtyWhse4");
        int cP4 = FindColumn(ws, headerRow, "OnPOWhse4");
        int cA7 = FindColumn(ws, headerRow, "AvailQtyWhse7");
        int cP7 = FindColumn(ws, headerRow, "OnPOWhse7");
        int cDesc = FindColumn(ws, headerRow, "ProductDescription");
        int cCasePack = FindColumn(ws, headerRow, "VendorPackageQuantity");

        var effective = FileNameDateParser.ExtractDateFromFileName(path);
        var rows = new List<BuyerInventoryRow>(20000);

        var lastRow = ws.LastRowUsed()?.RowNumber() ?? headerRow;
        for (int rnum = headerRow + 1; rnum <= lastRow; rnum++)
        {
            var r = ws.Row(rnum);

            var rawUpc = r.Cell(colUpc).GetString();
            if (!UpcNormalizer.TryNormalizeTo10(rawUpc, out var upc, out _)) continue;

            int ytd = (int)r.Cell(colYTD).GetDoubleOrDefault();
            int lastYear = (int)r.Cell(colPrior).GetDoubleOrDefault();
            int casePack = (int)r.Cell(cCasePack).GetDoubleOrDefault(); 

            int onHand = (int)(
                  r.Cell(cA8).GetDoubleOrDefault()
                + r.Cell(cA4).GetDoubleOrDefault()
                + r.Cell(cA7).GetDoubleOrDefault()
            );

            int onPo = (int)(
                  r.Cell(cP8).GetDoubleOrDefault()
                + r.Cell(cP4).GetDoubleOrDefault()
                + r.Cell(cP7).GetDoubleOrDefault()
            );

            var desc = r.Cell(cDesc).GetString().Trim();

            rows.Add(new BuyerInventoryRow
            {
                BuyerId = _buyerId,
                Upc = upc,
                CasePack = casePack,
                Description = desc,
                OnHand = onHand,
                OnPo = onPo,
                SalesYTD = ytd,
                UnitsSoldLastYear = lastYear,
                EffectiveDate = DateOnly.FromDateTime(effective)
            });
        }

        await _repo.LoadAsync(rows, ct);
    }
}