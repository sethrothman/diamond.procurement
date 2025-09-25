using System.Data;
using Dapper;
using Diamond.Procurement.Domain.Models;

namespace Diamond.Procurement.Data;

public sealed class VendorForecastRepository
{
    private readonly IDbFactory _dbf;
    public VendorForecastRepository(IDbFactory dbf) => _dbf = dbf;

    public async Task LoadAsync(IEnumerable<VendorForecastRow> rows, CancellationToken ct)
    {
        using var db = _dbf.Create();
        var tvp = BuildTvp(rows);
        var p = new DynamicParameters();
        p.Add("@Rows", tvp.AsTableValuedParameter("dbo.VendorForecastType"));
        await db.ExecuteAsync(new CommandDefinition("dbo.VendorForecast_Load", p, commandType: CommandType.StoredProcedure, cancellationToken: ct));
    }

    private static DataTable BuildTvp(IEnumerable<VendorForecastRow> rows)
    {
        var dt = new DataTable();
        dt.Columns.Add("Upc", typeof(string));
        dt.Columns.Add("Description", typeof(string));
        dt.Columns.Add("CasePack", typeof(int));
        dt.Columns.Add("VendorId", typeof(int));
        dt.Columns.Add("Price", typeof(decimal));
        dt.Columns.Add("QtyInCases", typeof(int));
        dt.Columns.Add("EffectiveDate", typeof(DateTime));

        foreach (var r in rows)
        {
            var dr = dt.NewRow();
            dr["Upc"] = r.Upc;
            dr["Description"] = (r.Description ?? string.Empty).Trim();
            dr["CasePack"] = r.CasePack;
            dr["VendorId"] = r.VendorId;
            dr["Price"] = r.Price;
            dr["QtyInCases"] = r.QtyInCases;
            dr["EffectiveDate"] = r.EffectiveDate.ToDateTime(TimeOnly.MinValue);
            dt.Rows.Add(dr);
        }
        return dt;
    }
}
