using System.Data;
using Dapper;
using Diamond.Procurement.Domain.Models;

namespace Diamond.Procurement.Data;

public sealed class BuyerInventoryRepository
{
    private static readonly DataTableBuilder<BuyerInventoryRow> BuyerInventoryTvpBuilder =
        new DataTableBuilder<BuyerInventoryRow>()
            .AddColumn("Upc", r => r.Upc)
            .AddColumn("Description", r => (r.Description ?? string.Empty).Trim())
            .AddColumn("CasePack", r => r.CasePack)
            .AddColumn("BuyerId", r => r.BuyerId)
            .AddColumn("OnHand", r => r.OnHand)
            .AddColumn("OnPo", r => r.OnPo)
            .AddColumn("SalesYTD", r => r.SalesYTD)
            .AddColumn("UnitsSoldLastYear", r => r.UnitsSoldLastYear)
            .AddColumn("EffectiveDate", r => r.EffectiveDate.ToDateTime(TimeOnly.MinValue))
            .AddColumn("StrikePrice", r => r.StrikePrice);

    private readonly IDbFactory _dbf;
    public BuyerInventoryRepository(IDbFactory dbf) => _dbf = dbf;

    public async Task LoadAsync(IEnumerable<BuyerInventoryRow> rows, CancellationToken ct)
    {
        using var db = _dbf.Create();
        var tvp = BuildTvp(rows);
        var p = new DynamicParameters();
        p.Add("@Rows", tvp.AsTableValuedParameter("dbo.BuyerInventoryType"));
        await db.ExecuteAsync(new CommandDefinition("dbo.BuyerInventory_Load", p, commandType: CommandType.StoredProcedure, cancellationToken: ct));
    }

    private static DataTable BuildTvp(IEnumerable<BuyerInventoryRow> rows) =>
        BuyerInventoryTvpBuilder.Build(rows);

    public async Task<IReadOnlyList<BuyerSoldVendorNotInMasterRow>> ListBuyerSoldVendorNotInMasterAsync(CancellationToken ct = default)
    {
        const string sql = @"
            SELECT u.Upc, u.Description, (bi.OnHand + bi.OnPo)/u.CasePack AS BuyerInventory, bi.UnitsSoldLastYear/u.CasePack AS SoldLastYear
                , bi.SalesYTD/u.CasePack AS SoldYtd, vf.QtyInCases AS VendorQty
            FROM dbo.BuyerInventory bi
            INNER JOIN dbo.Upc u
	            ON u.UpcId = bi.UpcId	
            INNER JOIN dbo.VendorForecast vf
	            ON vf.UpcId = u.UpcId   
            WHERE NOT EXISTS (SELECT * FROM dbo.MasterListDetail mi WHERE mi.UpcId = bi.UpcId)
            --WHERE NOT EXISTS (SELECT * FROM dbo.MainframeInventory mi WHERE mi.UpcId = bi.UpcId)
	            AND bi.SalesYTD > 0
            ";

        using var db = _dbf.Create();
        // Dapper doesn't support CancellationToken on QueryAsync<T> directly for SqlClient.
        var rows = await db.QueryAsync<BuyerSoldVendorNotInMasterRow>(sql);

        return rows.AsList();
    }
}
