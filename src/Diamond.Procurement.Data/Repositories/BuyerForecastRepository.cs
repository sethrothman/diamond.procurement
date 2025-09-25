using System.Data;
using Dapper;
using Diamond.Procurement.Domain.Models;

namespace Diamond.Procurement.Data;

public sealed class BuyerForecastRepository
{
    private readonly IDbFactory _dbf;
    public BuyerForecastRepository(IDbFactory dbf) => _dbf = dbf;

    public async Task LoadAsync(IEnumerable<BuyerForecastRow> rows, CancellationToken ct)
    {
        using var db = _dbf.Create();
        var tvp = BuildTvp(rows);
        var p = new DynamicParameters();
        p.Add("@Rows", tvp.AsTableValuedParameter("dbo.BuyerForecastType"));
        await db.ExecuteAsync(new CommandDefinition("dbo.BuyerForecast_Load", p, commandType: CommandType.StoredProcedure, cancellationToken: ct));
    }

    private static DataTable BuildTvp(IEnumerable<BuyerForecastRow> rows)
    {
        var dt = new DataTable();
        dt.Columns.Add("Upc", typeof(string));
        dt.Columns.Add("Description", typeof(string));          
        dt.Columns.Add("BuyerId", typeof(int));
        dt.Columns.Add("QtyInUnits", typeof(int));
        dt.Columns.Add("EffectiveDate", typeof(DateTime));

        foreach (var r in rows)
        {
            var dr = dt.NewRow();
            dr["Upc"] = r.Upc;
            dr["Description"] = (r.Description ?? string.Empty).Trim();  // NEW
            dr["BuyerId"] = r.BuyerId;
            dr["QtyInUnits"] = r.QtyInUnits;
            dr["EffectiveDate"] = r.ForecastDate.ToDateTime(TimeOnly.MinValue);
            dt.Rows.Add(dr);
        }
        return dt;
    }
}
