namespace Diamond.Procurement.Domain.Models;

public sealed class BuyerForecastRow
{
    public int BuyerId { get; set; }
    public string Upc { get; set; } = "";
    public string? Description { get; set; }
    public int QtyInUnits { get; set; }
    public DateOnly ForecastDate { get; set; }
}
