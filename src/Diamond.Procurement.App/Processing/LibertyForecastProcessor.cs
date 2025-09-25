using ClosedXML.Excel;
using Diamond.Procurement.Data;
using Diamond.Procurement.Domain.Models;
using Diamond.Procurement.Domain.Util;
using Diamond.Procurement.App.Util; 

namespace Diamond.Procurement.App.Processing;

public sealed class LibertyForecastProcessor : BaseExcelProcessor, IFileProcessor
{
    private readonly VendorForecastRepository _repo;
    private readonly MasterListRepository _masterListRepo;
    private int _vendorId;

    public FileKind Kind => FileKind.VendorForecast;

    public LibertyForecastProcessor(Data.VendorForecastRepository repo, MasterListRepository masterListRepo, int vendorId = 1)
    {
        _repo = repo;
        _masterListRepo = masterListRepo;
        _vendorId = vendorId;
    }

    public async Task SetVendor(int masterListId, CancellationToken ct) 
    {
        var row = await _masterListRepo.GetByIdAsync(masterListId, ct);
        _vendorId = row.VendorId; 
    }
        public async Task ProcessAsync(string path, CancellationToken ct)
    {
        if (_vendorId <= 0) throw new InvalidOperationException("VendorId not set.");

        using var wb = new XLWorkbook(path);
        var ws = wb.Worksheets.First();
        var headerRow = ForecastHeaderHelpers.FindVendorHeaderRow(ws);

        int colUpc = FindColumn(ws, headerRow, "Item UPC");
        int colPrice = FindColumn(ws, headerRow, "Price per Case");
        int colQty = FindColumn(ws, headerRow, "Forecast");
        int colDesc = FindColumn(ws, headerRow, "EMD");
        int colCasePack = FindColumn(ws, headerRow, "Case QTY", "Case Pack");

        var eff = TodayDateOnly();
        var rows = new List<VendorForecastRow>(20000);

        // iterate by row number
        var lastRow = ws.LastRowUsed()?.RowNumber() ?? headerRow;
        for (int rnum = headerRow + 1; rnum <= lastRow; rnum++)
        {
            var r = ws.Row(rnum);

            var rawUpcFromSheet = r.Cell(colUpc).GetString();

            // NEW: massage xxxxx-xxxxxEA -> digits only, then normalize
            var cleaned = ForecastHeaderHelpers.CleanLibertyUpc(rawUpcFromSheet);

            if (!UpcNormalizer.TryNormalizeTo10(cleaned, out var upc, out _))
                continue;

            var desc = r.Cell(colDesc).GetString().Trim();

            // Use your existing extension; cast to decimal/int as needed
            var price = (decimal)r.Cell(colPrice).GetDoubleOrDefault();
            var qty = (int)r.Cell(colQty).GetDoubleOrDefault();
            var casePack = (int)r.Cell(colCasePack).GetDoubleOrDefault();

            rows.Add(new VendorForecastRow
            {
                VendorId = _vendorId,
                Description = desc,
                CasePack = casePack,
                Upc = upc,
                Price = price,
                QtyInCases = qty,            
                EffectiveDate = eff
            });
        }

        await _repo.LoadAsync(rows, ct);
    }
}