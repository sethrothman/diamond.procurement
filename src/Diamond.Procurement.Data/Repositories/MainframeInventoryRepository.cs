using System.Data;
using Dapper;
using Diamond.Procurement.Domain.Models;

namespace Diamond.Procurement.Data;

public sealed class MainframeInventoryRepository
{
    private static readonly DataTableBuilder<MainframeInventoryRow> MainframeInventoryTvpBuilder =
        new DataTableBuilder<MainframeInventoryRow>()
            .AddColumn("Upc", r => r.Upc)
            .AddColumn("Description", r => (r.Description ?? string.Empty).Trim())
            .AddColumn("CasePack", r => r.CasePack)
            .AddColumn("QtyAvailable", r => r.QtyAvailable)
            .AddColumn("QtyOnPo", r => r.QtyOnPo)
            .AddColumn("QtyOverstock", r => r.QtyOverstock)
            .AddColumn("ListPrice", r => r.ListPrice)
            .AddColumn("EffectiveDate", r => r.EffectiveDate.ToDateTime(TimeOnly.MinValue))
            .AddColumn("HI", r => r.HI)
            .AddColumn("TI", r => r.TI);

    private readonly IDbFactory _dbf;
    public MainframeInventoryRepository(IDbFactory dbf) => _dbf = dbf;

    public async Task LoadAsync(IEnumerable<MainframeInventoryRow> rows, CancellationToken ct)
    {
        using var db = _dbf.Create();
        var tvp = BuildTvp(rows);
        var p = new DynamicParameters();
        p.Add("@Rows", tvp.AsTableValuedParameter("dbo.MainframeInventoryType"));
        await db.ExecuteAsync(new CommandDefinition(
            "dbo.MainframeInventory_Load",
            p,
            commandType: CommandType.StoredProcedure,
            cancellationToken: ct,
            commandTimeout: 300));
    }

    private static DataTable BuildTvp(IEnumerable<MainframeInventoryRow> rows) =>
        MainframeInventoryTvpBuilder.Build(rows);
}
