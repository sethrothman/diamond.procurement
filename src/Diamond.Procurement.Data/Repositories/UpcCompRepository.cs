using System.Data;
using Dapper;
using Diamond.Procurement.Domain.Models;

namespace Diamond.Procurement.Data
{
    public sealed class UpcCompRepository
    {
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

        private static DataTable BuildTvp(IEnumerable<UpcCompRow> rows)
        {
            var dt = new DataTable();
            dt.Columns.Add("Upc", typeof(string));
            dt.Columns.Add("PriceVictory", typeof(decimal));
            dt.Columns.Add("QtyVictory", typeof(int));
            dt.Columns.Add("PriceQualityKing", typeof(decimal));
            dt.Columns.Add("QtyQualityKing", typeof(int));

            foreach (var r in rows)
            {
                var dr = dt.NewRow();
                dr["Upc"] = r.Upc;
                dr["PriceVictory"] = r.PriceVictory.HasValue ? r.PriceVictory.Value : DBNull.Value;
                dr["QtyVictory"] = r.QtyVictory.HasValue ? r.QtyVictory.Value : DBNull.Value;
                dr["PriceQualityKing"] = r.PriceQualityKing.HasValue ? r.PriceQualityKing.Value : DBNull.Value;
                dr["QtyQualityKing"] = r.QtyQualityKing.HasValue ? r.QtyQualityKing.Value : DBNull.Value;
                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}
