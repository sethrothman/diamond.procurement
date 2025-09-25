// Domain/Contracts
public interface IImportStatusRepository
{
    // Existing rollup (kept for the other 4 file types)
    Task<ImportLastUpdates> GetLastUpdatesAsync(CancellationToken ct = default);

    // NEW: detailed, per-buyer Buyer Inventory last imports (based on SysStartTime → local)
    Task<IReadOnlyList<BuyerInventoryLastUpdate>> GetBuyerInventoryLastUpdatesAsync(CancellationToken ct = default);
}

public sealed record ImportLastUpdates(
    DateOnly? BuyerInventory,   // kept for backward-compat (we’ll display a rollup)
    DateOnly? BuyerForecast,
    DateOnly? VendorForecast,
    DateOnly? MainframeInventory,
    DateOnly? UpcComp
);

// NEW: per-buyer row used by the tooltip
public sealed record BuyerInventoryLastUpdate(
    int BuyerId,
    string BuyerName,
    DateTime LocalTime  // local time (converted from SysStartTime)
);
