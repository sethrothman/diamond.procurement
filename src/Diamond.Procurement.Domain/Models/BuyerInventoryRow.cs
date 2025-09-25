namespace Diamond.Procurement.Domain.Models;

public sealed class BuyerInventoryRow
{
    public int BuyerId { get; set; }
    public string Upc { get; set; } = "";
    public int CasePack { get; set; }
    public string? Description { get; set; }
    public int OnHand { get; set; }
    public int OnPo { get; set; }
    public int UnitsSoldLastYear { get; set; }
    public int SalesYTD { get; set; }
    public DateOnly EffectiveDate { get; set; }
    public decimal? StrikePrice { get; set; }  // money NULL
}
