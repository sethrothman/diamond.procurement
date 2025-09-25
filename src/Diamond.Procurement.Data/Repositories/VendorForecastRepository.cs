using System.Data;
using Dapper;
using Diamond.Procurement.Domain.Models;

namespace Diamond.Procurement.Data;

public sealed class VendorForecastRepository
{
    private static readonly DataTableBuilder<VendorForecastRow> VendorForecastTvpBuilder =
        new DataTableBuilder<VendorForecastRow>()
            .AddColumn("Upc", r => r.Upc)
            .AddColumn("Description", r => (r.Description ?? string.Empty).Trim())
            .AddColumn("CasePack", r => r.CasePack)
            .AddColumn("VendorId", r => r.VendorId)
            .AddColumn("Price", r => r.Price)
            .AddColumn("QtyInCases", r => r.QtyInCases)
            .AddColumn("EffectiveDate", r => r.EffectiveDate.ToDateTime(TimeOnly.MinValue));

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

    private static DataTable BuildTvp(IEnumerable<VendorForecastRow> rows) =>
        VendorForecastTvpBuilder.Build(rows);
}
