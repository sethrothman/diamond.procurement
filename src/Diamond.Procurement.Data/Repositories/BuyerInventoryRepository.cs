using System.Data;
using Dapper;
using Diamond.Procurement.Domain.Models;

namespace Diamond.Procurement.Data;

public sealed class BuyerInventoryRepository
{
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

    private static DataTable BuildTvp(IEnumerable<BuyerInventoryRow> rows)
    {
        var dt = new DataTable();
        dt.Columns.Add("Upc", typeof(string));
        dt.Columns.Add("Description", typeof(string));          
        dt.Columns.Add("CasePack", typeof(int));
        dt.Columns.Add("BuyerId", typeof(int));
        dt.Columns.Add("OnHand", typeof(int));
        dt.Columns.Add("OnPo", typeof(int));
        dt.Columns.Add("SalesYTD", typeof(int));
        dt.Columns.Add("UnitsSoldLastYear", typeof(int));
        dt.Columns.Add("EffectiveDate", typeof(DateTime));
        dt.Columns.Add("StrikePrice", typeof(decimal));  // money (nullable)

        foreach (var r in rows)
        {
            var dr = dt.NewRow();
            dr["Upc"] = r.Upc;
            dr["CasePack"] = r.CasePack;
            dr["Description"] = (r.Description ?? string.Empty).Trim();  
            dr["BuyerId"] = r.BuyerId;
            dr["OnHand"] = r.OnHand;
            dr["OnPo"] = r.OnPo;
            dr["SalesYTD"] = r.SalesYTD;
            dr["UnitsSoldLastYear"] = r.UnitsSoldLastYear;
            dr["EffectiveDate"] = r.EffectiveDate.ToDateTime(TimeOnly.MinValue);
            dr["StrikePrice"] = r.StrikePrice is null ? (object)DBNull.Value : r.StrikePrice.Value;
            dt.Rows.Add(dr);
        }
        return dt;
    }

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
