namespace Diamond.Procurement.Domain.Models
{
    public sealed class UpcCompRow
    {
        public string Upc { get; set; } = "";
        public decimal? PriceVictory { get; set; }
        public int? QtyVictory { get; set; }
        public decimal? PriceQualityKing { get; set; }
        public int? QtyQualityKing { get; set; }
    }
}
