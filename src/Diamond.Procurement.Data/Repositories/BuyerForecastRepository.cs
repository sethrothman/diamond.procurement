using System.Data;
using Dapper;
using Diamond.Procurement.Domain.Models;

namespace Diamond.Procurement.Data;

public sealed class BuyerForecastRepository
{
    private static readonly DataTableBuilder<BuyerForecastRow> BuyerForecastTvpBuilder =
        new DataTableBuilder<BuyerForecastRow>()
            .AddColumn("Upc", r => r.Upc)
            .AddColumn("Description", r => (r.Description ?? string.Empty).Trim())
            .AddColumn("BuyerId", r => r.BuyerId)
            .AddColumn("QtyInUnits", r => r.QtyInUnits)
            .AddColumn("EffectiveDate", r => r.ForecastDate.ToDateTime(TimeOnly.MinValue));

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

    private static DataTable BuildTvp(IEnumerable<BuyerForecastRow> rows) =>
        BuyerForecastTvpBuilder.Build(rows);
}
