using System;
using System.Data;
using Dapper;
using Diamond.Procurement.Data.Models;

namespace Diamond.Procurement.Data.Repositories;

public interface IOrderVendorShipmentRepository
{
    Task<IReadOnlyList<ShipmentListItem>> GetShipmentsAsync(int orderVendorId);
    Task<IReadOnlyList<UpcResolution>> ResolveUpcsAsync(IEnumerable<string> inputs);

    // NEW: create shipment + details atomically
    Task<int> InsertShipmentWithDetailsAsync(
        int orderVendorId,
        int truckNum,
        string InvoiceNum,
        DateTime shipmentDate,
        DateTime estimatedDeliveryDate,
        IEnumerable<(int UpcId, int Quantity)> details);

    Task<IReadOnlyList<ShipmentQuantityRow>> GetShipmentQuantitiesAsync(int orderVendorId);
}

public sealed class OrderVendorShipmentRepository : IOrderVendorShipmentRepository
{
    private static readonly DataTableBuilder<(int UpcId, int Quantity)> ShipmentDetailTvpBuilder =
        new DataTableBuilder<(int UpcId, int Quantity)>()
            .AddColumn("UpcId", d => d.UpcId)
            .AddColumn("Quantity", d => d.Quantity);

    private readonly IDbFactory _dbf;
    public OrderVendorShipmentRepository(IDbFactory dbf) => _dbf = dbf;

    public async Task<IReadOnlyList<ShipmentListItem>> GetShipmentsAsync(int orderVendorId)
    {
        const string sql = @"
            SELECT s.OrderVendorShipmentId, s.TruckNum, s.ShipmentDate, s.EstimatedDeliveryDate, s.InvoiceNum
            FROM dbo.OrderVendorShipment s
            WHERE s.OrderVendorId = @OrderVendorId
            ORDER BY s.ShipmentDate, s.OrderVendorShipmentId;";

        using var db = _dbf.Create();
        return (await db.QueryAsync<ShipmentListItem>(sql, new { OrderVendorId = orderVendorId })).ToList();
    }

    public async Task<IReadOnlyList<UpcResolution>> ResolveUpcsAsync(IEnumerable<string> inputs)
    {
        var list = inputs
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => new UpcResolution { Input = s.Trim(), Canonical = CanonicalizeUpc(s) })
            .ToList();

        if (list.Count == 0) return list;

        var canonSet = list.Select(x => x.Canonical).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

        using var db = _dbf.Create();
        var hits = await db.QueryAsync<(int UpcId, string Upc)>("SELECT UpcId, Upc FROM dbo.Upc WHERE Upc IN @Upcs", new { Upcs = canonSet });

        var map = hits.ToDictionary(t => t.Upc, t => t.UpcId, StringComparer.OrdinalIgnoreCase);

        foreach (var r in list)
            r.UpcId = map.TryGetValue(r.Canonical, out var id) ? id : (int?)null;

        return list;
    }

    private static string CanonicalizeUpc(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return "";
        var digits = new string(raw.Where(char.IsDigit).ToArray());

        // Example normalization: client 11-digit → DB "00" + 11 (adjust to your real rule)
        //if (digits.Length == 11) return "00" + digits;

        return digits;
    }

    // --- NEW: atomic insert of header + details ---

    public async Task<int> InsertShipmentWithDetailsAsync(
        int orderVendorId,
        int truckNum,
        string invoiceNum,
        DateTime shipmentDate,
        DateTime estimatedDeliveryDate,
        IEnumerable<(int UpcId, int Quantity)> details)
    {
        using var db = _dbf.Create();

        using var tvp = ShipmentDetailTvpBuilder.Build(details ?? Array.Empty<(int UpcId, int Quantity)>());

        var p = new DynamicParameters();
        p.Add("@OrderVendorId", orderVendorId);
        p.Add("@TruckNum", truckNum);
        p.Add("@InvoiceNum", invoiceNum);
        p.Add("@ShipmentDate", shipmentDate);
        p.Add("@EstimatedDeliveryDate", estimatedDeliveryDate);
        p.Add("@Rows", tvp.AsTableValuedParameter("dbo.OrderVendorShipmentDetailType"));

        var newId = await db.ExecuteScalarAsync<int>("dbo.OrderVendorShipment_InsertWithDetails", p, commandType: CommandType.StoredProcedure);

        return newId;
    }

    public async Task<IReadOnlyList<ShipmentQuantityRow>> GetShipmentQuantitiesAsync(int orderVendorId)
    {
        const string sql = @"
            SELECT s.OrderVendorShipmentId, d.UpcId, SUM(d.Quantity) AS Quantity
            FROM dbo.OrderVendorShipment s
            INNER JOIN dbo.OrderVendorShipmentDetail d
                ON d.OrderVendorShipmentId = s.OrderVendorShipmentId
            WHERE s.OrderVendorId = @OrderVendorId
            GROUP BY s.OrderVendorShipmentId, d.UpcId
            ORDER BY s.OrderVendorShipmentId, d.UpcId;";

        using var db = _dbf.Create();
        var rows = await db.QueryAsync<ShipmentQuantityRow>(sql, new { OrderVendorId = orderVendorId });
        return rows.ToList();
    }
}
