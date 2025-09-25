namespace Diamond.Procurement.Domain.Models;

public sealed class VendorOrderResult
{
    public int Updated { get; }
    public string SavedPath { get; }

    public VendorOrderResult(int updated, string savedPath)
    {
        Updated = updated;
        SavedPath = savedPath;
    }
}
