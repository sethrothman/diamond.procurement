using Dapper;
using Diamond.Procurement.Data.Contracts;

namespace Diamond.Procurement.Data.Repositories
{
    public sealed class FtpEndpointRepository : IFtpEndpointRepository
    {
        private readonly IDbFactory _dbf;
        public FtpEndpointRepository(IDbFactory dbf) => _dbf = dbf;

        public async Task<FtpEndpoint?> GetActiveAsync(CancellationToken ct = default)
        {
            using var db = _dbf.Create();

            const string sql = @"
                SELECT TOP(1) *
                FROM dbo.FtpEndpoint
                WHERE IsActive = 1
                ORDER BY FtpEndpointId DESC;";

            var cmd = new CommandDefinition(sql, cancellationToken: ct);
            return await db.QueryFirstOrDefaultAsync<FtpEndpoint>(cmd);
        }
    }
}
