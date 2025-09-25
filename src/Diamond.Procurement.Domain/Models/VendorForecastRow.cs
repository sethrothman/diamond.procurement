namespace Diamond.Procurement.Domain.Models;

public sealed class VendorForecastRow
{
    public int VendorId { get; set; }
    public string Upc { get; set; } = "";
    public string? Description { get; set; }
    public int CasePack { get; set; }
    public decimal Price { get; set; }
    public int QtyInCases { get; set; }
    public DateOnly EffectiveDate { get; set; }
}
