using Diamond.Procurement.Domain.Enums;

namespace Diamond.Procurement.Domain.Models;

public sealed class MasterListRow
{
    // Null/0 means "new" (insert); >0 means "update this PK"
    public int? MasterListId { get; set; }
    public string? Name { get; set; }
    public ListTypeId ListTypeId { get; set; }
    public int BuyerId { get; set; }
    public int VendorId { get; set; }
}

public sealed class MasterListSummary
{
    public int MasterListId { get; set; }
    public string Name { get; set; } = "";
    public string VendorName { get; set; } = "";
    public string BuyerName { get; set; } = "";
    public int ListTypeId { get; set; }
}

