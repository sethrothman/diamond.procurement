// Data
using Dapper;
using Diamond.Procurement.Data;

public sealed class ImportStatusRepository : IImportStatusRepository
{
    private readonly IDbFactory _dbf;
    public ImportStatusRepository(IDbFactory dbf) => _dbf = dbf;

    public async Task<ImportLastUpdates> GetLastUpdatesAsync(CancellationToken ct = default)
    {
        using var db = _dbf.Create();

        // NOTE: We only need the first row because you delete+reinsert on each upload.
        // Still, use ORDER BY DESC to be safe and NOLOCK to avoid blocking.
        const string sql = @"
            SELECT TOP (1) SysStartTime FROM dbo.BuyerInventory WITH (NOLOCK) ORDER BY SysStartTime DESC;
            SELECT TOP (1) SysStartTime FROM dbo.BuyerForecast  WITH (NOLOCK) ORDER BY SysStartTime DESC;
            SELECT TOP (1) SysStartTime FROM dbo.VendorForecast WITH (NOLOCK) ORDER BY SysStartTime DESC;
            SELECT TOP (1) EffectiveDate FROM dbo.MainframeInventory WITH (NOLOCK) ORDER BY EffectiveDate DESC;
            SELECT TOP (1) EffectiveDate FROM dbo.UpcComp WITH (NOLOCK) ORDER BY EffectiveDate DESC;
";

        DateTime? bi = null, bf = null, vf = null;
        DateTime? mfi = null, uc = null;

        using (var multi = await db.QueryMultipleAsync(new CommandDefinition(sql, cancellationToken: ct)))
        {
            bi = await multi.ReadFirstOrDefaultAsync<DateTime?>();
            bf = await multi.ReadFirstOrDefaultAsync<DateTime?>();
            vf = await multi.ReadFirstOrDefaultAsync<DateTime?>();
            mfi = await multi.ReadFirstOrDefaultAsync<DateTime?>();
            uc = await multi.ReadFirstOrDefaultAsync<DateTime?>();
        }

        // SysStartTime from temporal tables is effectively UTC; convert to local and take DateOnly.
        static DateOnly? ToLocalDateOnly(DateTime? dt)
        {
            if (dt is null) return null;
            // Dapper maps datetime2 as Kind=Unspecified. Treat as UTC then convert.
            var utc = DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc);
            var local = utc.ToLocalTime();
            return DateOnly.FromDateTime(local);
        }

        // MainframeInventory.EffectiveDate is a date (no time) – interpret as local date directly.
        static DateOnly? FromDateOnly(DateTime? date) => date is null ? null : DateOnly.FromDateTime(date.Value);

        return new ImportLastUpdates(
            BuyerInventory: ToLocalDateOnly(bi),
            BuyerForecast: ToLocalDateOnly(bf),
            VendorForecast: ToLocalDateOnly(vf),
            MainframeInventory: FromDateOnly(mfi),
            UpcComp: FromDateOnly(uc)
        );
    }

    public async Task<IReadOnlyList<BuyerInventoryLastUpdate>> GetBuyerInventoryLastUpdatesAsync(CancellationToken ct = default)
    {
        using var db = _dbf.Create();

        // If BuyerInventory is temporal, SysStartTime is available.
        // We group by BuyerId to get the latest per buyer.
        const string sql = @"
            SELECT bi.BuyerId, b.BuyerName,
                   MAX(bi.SysStartTime) AS LastSysStartTime
            FROM dbo.BuyerInventory bi WITH (NOLOCK)
            INNER JOIN dbo.Buyer b 
                ON b.BuyerId = bi.BuyerId
            GROUP BY bi.BuyerId, b.BuyerName
            ORDER BY b.BuyerName;";

        var rows = await db.QueryAsync<(int BuyerId, string BuyerName, DateTime? LastSysStartTime)>(
            new CommandDefinition(sql, cancellationToken: ct));

        static DateTime ToLocal(DateTime? dt)
        {
            if (dt is null) return DateTime.MinValue; // never shown if null; guard
            // Dapper often maps datetime2 as Kind=Unspecified ⇒ treat as UTC then convert.
            var utc = DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc);
            return utc.ToLocalTime();
        }

        var list = rows
            .Where(r => r.LastSysStartTime.HasValue)
            .Select(r => new BuyerInventoryLastUpdate(r.BuyerId, r.BuyerName, ToLocal(r.LastSysStartTime)))
            .ToList()
            .AsReadOnly();

        return list;
    }
}
