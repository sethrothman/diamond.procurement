using Dapper;
using Diamond.Procurement.Data.Models;
using Diamond.Procurement.Domain.Models;
using System.Data;

namespace Diamond.Procurement.Data;

public interface IOrderVendorRepository
{
    //Task<(int orderVendorId, DateTime orderMonth)> EnsureAndSeedAsync(int vendorId, DateTime anyDate, IEnumerable<OrderSeedRow> seeds, CancellationToken ct);

    Task<IReadOnlyList<OrderMonthOption>> ListMonthsAsync(CancellationToken ct);

    Task<IReadOnlyList<VendorOrderGridRow>> GetGridAsync(int masterListId, int orderVendorId, DateTime forecastFromDate, DateTime forecastThruDate, CancellationToken ct, bool seedData = false);

    Task<int> UpdateQuantitiesAsync(int orderVendorId, IEnumerable<OrderQtyEdit> rows, CancellationToken ct);
    Task<int> AddNewOrderMonthAsync(int masterListId, int orderVendorId);
    Task SeedData(int masterListId, int orderVendorId);

    Task<IReadOnlyList<MatrixBaseRow>> ListOrderVendorDetailForMatrixAsync(int orderVendorId, CancellationToken ct);
}

public sealed class OrderVendorRepository : IOrderVendorRepository
{
    private readonly IDbFactory _dbf;
    public OrderVendorRepository(IDbFactory dbf) => _dbf = dbf;

    public async Task<int> AddNewOrderMonthAsync(int masterListId, int orderVendorId)
    {
        using var db = _dbf.Create();
        // Returns the newly inserted id — or the existing one if it already existed
        int newOrderVendorId = await db.QuerySingleAsync<int>(
            "dbo.OrderVendor_AddNextMonth",
            new { MasterListId = masterListId, OrderVendorId = orderVendorId },
            commandType: CommandType.StoredProcedure);

        return newOrderVendorId;
    }

    public async Task<IReadOnlyList<OrderMonthOption>> ListMonthsAsync(CancellationToken ct)
    {
        using var db = _dbf.Create();
        var rows = await db.QueryAsync<OrderMonthOption>(
            new CommandDefinition("dbo.OrderVendor_ListMonths", commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return rows.AsList();
    }

    public async Task<IReadOnlyList<VendorOrderGridRow>> GetGridAsync(int masterListId, int orderVendorId, DateTime forecastFromDate, DateTime forecastThruDate, CancellationToken ct, bool seedData = false)
    {
        if (seedData)
            await SeedData(masterListId, orderVendorId);

        using var db = _dbf.Create();
        var rows = await db.QueryAsync<VendorOrderGridRow>(
            new CommandDefinition("dbo.OrderVendor_Grid", new { OrderVendorId = orderVendorId, ForecastFromDate = forecastFromDate.Date, ForecastThruDate = forecastThruDate.Date },
                commandType: CommandType.StoredProcedure, cancellationToken: ct));
        return rows.AsList();
    }

    public async Task<int> UpdateQuantitiesAsync(int orderVendorId, IEnumerable<OrderQtyEdit> rows, CancellationToken ct)
    {
        using var db = _dbf.Create();
        var tvp = new DataTable();
        tvp.Columns.Add("UpcId", typeof(int));
        tvp.Columns.Add("QtyInCases", typeof(int));
        tvp.Columns.Add("ExtraCases", typeof(int));
        tvp.Columns.Add("QtyConfirmed", typeof(int));
        tvp.Columns.Add("WeeksToBuy", typeof(int));

        foreach (var r in rows) tvp.Rows.Add(r.UpcId, r.QtyInCases, r.ExtraCases, r.QtyConfirmed, r.WeeksToBuy);

        var p = new DynamicParameters();
        p.Add("@OrderVendorId", orderVendorId);
        p.Add("@Rows", tvp.AsTableValuedParameter("dbo.OrderQtyType"));

        return await db.ExecuteAsync(
            new CommandDefinition("dbo.OrderVendor_UpdateQuantities", p,
                commandType: CommandType.StoredProcedure, cancellationToken: ct));
    }

    public async Task SeedData(int masterListId, int orderVendorId)
    {
        using var db = _dbf.Create();
        var sql = @"
            UPDATE dbo.OrderVendor SET MasterListId = @MasterListId WHERE OrderVendorId = @OrderVendorId;

            INSERT INTO dbo.OrderVendorDetail ( OrderVendorId, UpcId, Price )
            SELECT @OrderVendorId, mld.UpcId, vf.Price 
            FROM dbo.MasterList ml
            INNER JOIN dbo.MasterListDetail mld
	            ON mld.MasterListId = ml.MasterListId
            INNER JOIN dbo.VendorForecast vf
                ON mld.UpcId = vf.UpcId
            WHERE mld.MasterListId = @MasterListId
                AND ISNULL(mld.IsActive, 0) = 1
	            AND NOT EXISTS(SELECT * FROM OrderVendorDetail ovd WHERE ovd.OrderVendorId = @OrderVendorId AND ovd.UpcId = mld.UpcId);
            ";

        await db.ExecuteAsync(sql, new { MasterListId = masterListId, OrderVendorId = orderVendorId });
    }

    public async Task<IReadOnlyList<MatrixBaseRow>> ListOrderVendorDetailForMatrixAsync(int orderVendorId, CancellationToken ct)
    {
        const string sql = @"
            SELECT ovd.UpcId,
                   u.Upc AS [UPC],
                   u.Description,
                   ovd.QtyInCases,
                   ovd.Price
            FROM dbo.OrderVendorDetail ovd
            JOIN dbo.Upc u ON u.UpcId = ovd.UpcId
            WHERE ovd.OrderVendorId = @OrderVendorId
                AND ovd.QtyInCases > 0
            ORDER BY u.Upc;";

        using var db = _dbf.Create();
        return (await db.QueryAsync<MatrixBaseRow>(sql, new { OrderVendorId = orderVendorId })).ToList();
    }
}
