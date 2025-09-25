using DocumentFormat.OpenXml.Office2013.Excel;

namespace Diamond.Procurement.Domain.Models;

public sealed class VendorOrderGridRow
{
    public int UpcId { get; init; }
    public string Upc { get; init; } = "";
    public string Description { get; init; } = "";
    public decimal? ListPrice { get; set; }
    public int CasePack { get; init; }
    public decimal AvailableInCasesBuyer { get; init; }
    public decimal OnPoInCasesBuyer { get; init; }
    public decimal ForecastQtyInCasesVendor { get; init; }
    public decimal WeeklyRunRate { get; init; }
    public decimal TotalInCasesBuyer { get; init; }
    public decimal WeeksCover { get; init; }
    public decimal WeeksCoverDiamond { get; init; }
    public decimal WeeksCoverTotal { get; init; }
    public decimal UnitsSoldLastYearInCases { get; init; }
    public decimal AvailableInCasesDiamond { get; init; }
    public decimal OnPoInCasesDiamond { get; init; }
    public decimal OverstockDiamond { get; set; }
    public decimal ForecastQtyInCasesBuyer { get; init; }

    // NEW: UpcComp columns from the LEFT JOIN (nullable)
    public decimal? PriceVictory { get; set; }
    public decimal? PriceQualityKing { get; set; }
    public int? QtyVictory { get; set; }
    public int? QtyQualityKing { get; set; }

    // From OrderVendorDetail
    public decimal? PriceSnapshot { get; init; } // null before seeding
    public int? QtyForThisMonth { get; init; }   // null before seeding
    public int? ExtraCases { get; set; }
    public bool HasAlternateBuyer { get; set; }
    public int? QtyConfirmed { get; set; }
    public int? WeeksToBuy { get; set; }
    public int? HI { get; set; }
    public int? TI { get; set; }

    public decimal? MeijerWeeklyRunRate { get; init; }
    public decimal? MeijerStrikePrice { get; init; }
    public decimal? DGWeeklyRunRate { get; init; }
    public decimal? DGStrikePrice { get; init; }
}

public sealed class OrderMonthOption
{
    public int OrderVendorId { get; init; }
    public DateTime OrderMonth { get; init; } // first-of-month
    public string? Vendor { get; set; }
    public string? FullMonthYear { get; set; }
    public int MasterListId { get; set; }
    public string? MasterList { get; set; }
    public string? DisplayForLookupEdit => $"{FullMonthYear} ({MasterList})";
}

// For seeding header+lines
public sealed class OrderSeedRow
{
    public int UpcId { get; init; }
    public decimal Price { get; init; }
}

// For quantity updates
public sealed class OrderQtyEdit
{
    public int UpcId { get; init; }
    public int QtyInCases { get; init; }
    public int ExtraCases { get; set; }
    public int QtyConfirmed { get; set; }
    public int WeeksToBuy { get; set; }
}
