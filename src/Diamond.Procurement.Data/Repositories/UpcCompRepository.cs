using System.Data;
using Dapper;
using Diamond.Procurement.Domain.Models;

namespace Diamond.Procurement.Data
{
    public sealed class UpcCompRepository
    {
        private static readonly DataTableBuilder<UpcCompRow> UpcCompTvpBuilder =
            new DataTableBuilder<UpcCompRow>()
                .AddColumn("Upc", r => r.Upc)
                .AddColumn("PriceVictory", r => r.PriceVictory)
                .AddColumn("QtyVictory", r => r.QtyVictory)
                .AddColumn("PriceQualityKing", r => r.PriceQualityKing)
                .AddColumn("QtyQualityKing", r => r.QtyQualityKing);

        private readonly IDbFactory _dbf;
        public UpcCompRepository(IDbFactory dbf) => _dbf = dbf;

        public async Task LoadAsync(IEnumerable<UpcCompRow> rows, CancellationToken ct)
        {
            using var db = _dbf.Create();
            var tvp = BuildTvp(rows);
            var p = new DynamicParameters();
            p.Add("@Rows", tvp, DbType.Object);

            var cmd = new CommandDefinition(
                "dbo.UpcComp_Load",
                p,
                commandType: CommandType.StoredProcedure,
                cancellationToken: ct);

            await db.ExecuteAsync(cmd);
        }

        private static DataTable BuildTvp(IEnumerable<UpcCompRow> rows) =>
            UpcCompTvpBuilder.Build(rows);
    }
}
