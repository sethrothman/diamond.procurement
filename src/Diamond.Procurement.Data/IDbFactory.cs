using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Diamond.Procurement.Data;

public interface IDbFactory
{
    IDbConnection Create();
}

public sealed class SqlDbFactory : IDbFactory
{
    private readonly string _cs;
    public SqlDbFactory(IConfiguration cfg) => _cs = cfg.GetConnectionString("Sql")!;
    public IDbConnection Create() => new SqlConnection(_cs);
}
