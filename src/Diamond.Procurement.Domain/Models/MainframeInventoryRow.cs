namespace Diamond.Procurement.Domain.Models;

public sealed class MainframeInventoryRow
{
    public string Upc { get; set; } = "";
    public string? Description { get; set; }          
    public int? CasePack { get; set; }
    public int QtyAvailable { get; set; }
    public int QtyOnPo { get; set; }
    public int QtyOverstock { get; set; }
    public decimal? ListPrice { get; set; }
    public DateOnly EffectiveDate { get; set; }
    public int HI { get; set; }
    public int TI { get; set; }
}
