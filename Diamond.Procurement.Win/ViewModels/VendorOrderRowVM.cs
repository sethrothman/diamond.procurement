using Diamond.Procurement.Domain.Models;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Diamond.Procurement.Win.ViewModels;

public sealed class VendorOrderRowVM : INotifyPropertyChanged
{
    // -------- Identity / Immutable-ish inputs --------
    public int UpcId { get; }
    public string Upc { get; }
    public string Description { get; }
    public int CasePack { get; set; }
    public decimal? ListPrice { get; set; }

    // -------- Analytics (read-only snapshots) --------
    public decimal AvailableInCasesBuyer { get; }
    public decimal OnPoInCasesBuyer { get; }
    public decimal ForecastQtyInCasesVendor { get; }
    public decimal WeeklyRunRate { get; }            // cases/week
    public decimal WeeksCover { get; }
    public decimal WeeksCoverDiamond { get; }
    public decimal WeeksCoverTotal { get; }
    public decimal UnitsSoldLastYearInCases { get; }
    public decimal AvailableInCasesDiamond { get; }
    public decimal OnPoInCasesDiamond { get; }
    public decimal OverstockDiamond { get; set; }
    public decimal ForecastQtyInCasesBuyer { get; }

    // Comp columns (nullable)
    public decimal? PriceVictory { get; set; }
    public decimal? PriceQualityKing { get; set; }
    public int? QtyVictory { get; set; }
    public int? QtyQualityKing { get; set; }

    // New Haircare “All” columns (per-buyer)
    public decimal? MeijerWeeklyRunRate { get; init; }    
    public decimal? MeijerStrikePrice { get; init; }    
    public decimal? DGWeeklyRunRate { get; init; }    
    public decimal? DGStrikePrice { get; init; }    

    // Snapshot from OrderVendorDetail
    public decimal Price { get; } // read-only in UI

    // -------- Editable inputs (backing fields + notifications) --------
    private int _qtyInCases;
    public int QtyInCases
    {
        get => _qtyInCases;
        set
        {
            if (_qtyInCases == value) return;
            _qtyInCases = value;
            IsDirty = true;
            OnPropertyChanged();
            RaiseDerived(nameof(LineTotal), nameof(PalletCountFromProposed), nameof(PalletCountActive));
        }
    }

    private int _extraCases;
    public int ExtraCases
    {
        get => _extraCases;
        set
        {
            if (_extraCases == value) return;
            _extraCases = value;
            IsDirty = true;
            OnPropertyChanged();
            RecalculateQtyInCases(); // also raises derived below
        }
    }

    private int? _qtyConfirmed;
    public int? QtyConfirmed
    {
        get => _qtyConfirmed;
        set
        {
            if (_qtyConfirmed == value) return;
            _qtyConfirmed = value;
            IsDirty = true;
            OnPropertyChanged();
            RaiseDerived(nameof(LineTotal));
        }
    }

    private int? _weeksToBuy;
    public int? WeeksToBuy
    {
        get => _weeksToBuy;
        set
        {
            if (_weeksToBuy == value) return;
            _weeksToBuy = value;
            IsDirty = true;
            OnPropertyChanged();
            RecalcProposedFromWeeks(); // also raises derived below
        }
    }

    private int? _hi;
    public int? HI
    {
        get => _hi;
        set
        {
            if (_hi == value) return;
            _hi = value;
            IsDirty = true;
            OnPropertyChanged();
            RaiseDerived(nameof(CasesPerPallet),
                         nameof(PalletCountFromWeeks),
                         nameof(PalletCountFromProposed),
                         nameof(PalletCountActive));
        }
    }

    private int? _ti;
    public int? TI
    {
        get => _ti;
        set
        {
            if (_ti == value) return;
            _ti = value;
            IsDirty = true;
            OnPropertyChanged();
            RaiseDerived(nameof(CasesPerPallet),
                         nameof(PalletCountFromWeeks),
                         nameof(PalletCountFromProposed),
                         nameof(PalletCountActive));
        }
    }

    // Haircare vs Cosmetics switch for pallet math
    private bool _useWeeksForPallets;
    /// <summary>
    /// true = Haircare (use ProposedQtyFromWeeks), false = Cosmetics (use ProposedQtyInCases)
    /// </summary>
    public bool UseWeeksForPallets
    {
        get => _useWeeksForPallets;
        set
        {
            if (_useWeeksForPallets == value) return;
            _useWeeksForPallets = value;
            OnPropertyChanged();
            RaiseDerived(nameof(PalletCountActive));
        }
    }

    public bool UseConfirmed { get; set; }
    public bool HasAlternateBuyer { get; set; }

    // -------- Proposal (computed & cached values) --------
    // These are set internally; we notify when they change.
    private int _availableQtyInCases;
    public int AvailableQtyInCases
    {
        get => _availableQtyInCases;
        private set
        {
            if (_availableQtyInCases == value) return;
            _availableQtyInCases = value;
            OnPropertyChanged();
            RaiseDerived(nameof(PalletCountFromProposed), nameof(PalletCountActive));
        }
    }

    private int _proposedQtyInCases;
    public int ProposedQtyInCases
    {
        get => _proposedQtyInCases;
        private set
        {
            if (_proposedQtyInCases == value) return;
            _proposedQtyInCases = value;
            OnPropertyChanged();
            RaiseDerived(nameof(LineTotalDesired),
                         nameof(PalletCountFromProposed),
                         nameof(PalletCountActive));
        }
    }

    private int _proposedQtyFromWeeks;
    public int ProposedQtyFromWeeks
    {
        get => _proposedQtyFromWeeks;
        private set
        {
            if (_proposedQtyFromWeeks == value) return;
            _proposedQtyFromWeeks = value;
            OnPropertyChanged();
            RaiseDerived(nameof(LineTotalFromWeeks),
                         nameof(PalletCountFromWeeks),
                         nameof(PalletCountActive));
        }
    }

    // -------- Derived totals (get-only; we raise when inputs change) --------
    public decimal LineTotalDesired => Price * ProposedQtyInCases;
    public decimal LineTotal => Price * (UseConfirmed ? (QtyConfirmed ?? 0) : QtyInCases);
    public decimal ExtraLineTotal => Price * ExtraCases;

    public decimal LineTotalFromWeeks => Price * ProposedQtyFromWeeks;

    // Cases that fit on a single pallet (HI * TI). 0 if unknown/invalid.
    public int CasesPerPallet
    {
        get
        {
            var hi = _hi ?? 0;
            var ti = _ti ?? 0;
            var cpp = hi * ti;
            return cpp > 0 ? cpp : 0;
        }
    }

    public decimal PalletCountFromProposed => CalcPallets(ProposedQtyInCases);
    public decimal PalletCountFromWeeks => CalcPallets(ProposedQtyFromWeeks);

    /// <summary>
    /// Active pallet count based on mode:
    /// Haircare = weeks-driven; Cosmetics = manual proposed.
    /// </summary>
    public decimal PalletCountActive => _useWeeksForPallets ? PalletCountFromWeeks : PalletCountFromProposed;

    public int RequiredPalletsCeiling => CasesPerPallet <= 0 ? 0 : (int)Math.Ceiling(PalletCountActive);

    public decimal WeeklyRunRateAll =>
        WeeklyRunRate
        + (MeijerWeeklyRunRate ?? 0m)
        + (DGWeeklyRunRate ?? 0m);

    // 0..1 (fraction of the last pallet); multiply by 100 for percent
    public decimal LastPalletFillFraction
    {
        get
        {
            var frac = PalletCountActive - Math.Truncate(PalletCountActive);
            return frac < 0 ? 0m : frac;
        }
    }


    public bool IsDirty { get; set; }

    // -------- ctor --------
    public VendorOrderRowVM(VendorOrderGridRow r)
    {
        UpcId = r.UpcId;
        Upc = r.Upc;
        Description = r.Description;
        CasePack = r.CasePack;

        AvailableInCasesBuyer = r.AvailableInCasesBuyer;
        OnPoInCasesBuyer = r.OnPoInCasesBuyer;
        ForecastQtyInCasesVendor = r.ForecastQtyInCasesVendor;
        WeeklyRunRate = r.WeeklyRunRate;
        WeeksCover = r.WeeksCover;
        WeeksCoverDiamond = r.WeeksCoverDiamond;
        OverstockDiamond = r.OverstockDiamond;
        WeeksCoverTotal = r.WeeksCoverTotal;
        UnitsSoldLastYearInCases = r.UnitsSoldLastYearInCases;
        AvailableInCasesDiamond = r.AvailableInCasesDiamond;
        OnPoInCasesDiamond = r.OnPoInCasesDiamond;
        ForecastQtyInCasesBuyer = r.ForecastQtyInCasesBuyer;

        Price = r.PriceSnapshot ?? 0m;
        QtyInCases = r.QtyForThisMonth ?? 0;

        // "Desired" proposal (your existing math)
        var desired = (int)Math.Max(0, Math.Ceiling(r.ForecastQtyInCasesBuyer - (r.AvailableInCasesDiamond + r.OnPoInCasesDiamond)));
        ProposedQtyInCases = desired;
        AvailableQtyInCases = (int)Math.Min(desired, r.ForecastQtyInCasesVendor);

        ExtraCases = r.ExtraCases ?? 0;
        HasAlternateBuyer = r.HasAlternateBuyer;
        QtyConfirmed = r.QtyConfirmed;

        ListPrice = r.ListPrice;
        PriceVictory = r.PriceVictory;
        QtyVictory = r.QtyVictory;
        PriceQualityKing = r.PriceQualityKing;
        QtyQualityKing = r.QtyQualityKing;

        WeeksToBuy = r.WeeksToBuy;
        ProposedQtyFromWeeks = (int)Math.Ceiling((r.WeeksToBuy ?? 0) * r.WeeklyRunRate);

        TI = r.TI;
        HI = r.HI;

        MeijerStrikePrice = r.MeijerStrikePrice;
        MeijerWeeklyRunRate = r.MeijerWeeklyRunRate;
        DGStrikePrice = r.DGStrikePrice;
        DGWeeklyRunRate = r.DGWeeklyRunRate;        

        // default mode: Haircare uses weeks; flip to false for Cosmetics when you bind the page
        UseWeeksForPallets = true;
    }

    // -------- INotifyPropertyChanged --------
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void RaiseDerived(params string[] names)
    {
        foreach (var n in names) OnPropertyChanged(n);
    }

    // -------- Helpers / Recalc fan-out --------
    /// <summary>QtyInCases is Available + Extra.</summary>
    private void RecalculateQtyInCases()
    {
        QtyInCases = AvailableQtyInCases + ExtraCases;
        // QtyInCases setter already raises LineTotal + pallet props.
        RaiseDerived(nameof(ExtraLineTotal));
    }

    private void RecalcProposedFromWeeks()
    {
        if (WeeklyRunRate <= 0 || !_weeksToBuy.HasValue)
        {
            ProposedQtyFromWeeks = 0;
        }
        else
        {
            var qty = (decimal)_weeksToBuy.Value * WeeklyRunRate;
            ProposedQtyFromWeeks = (int)Math.Ceiling(qty);
        }
        // ProposedQtyFromWeeks setter raises its own derived props.
    }

    private decimal CalcPallets(int qty)
    {
        var cpp = CasesPerPallet;
        if (qty <= 0 || cpp <= 0) return 0m;
        return (decimal)qty / cpp;
    }

    // -------- Existing CSV builder (unchanged) --------
    public static string BuildMainframeCsv(IEnumerable<VendorOrderRowVM> rows)
    {
        var sb = new StringBuilder();
        sb.AppendLine("',',','");

        foreach (var r in rows.Where(x => x.QtyConfirmed.HasValue && x.QtyConfirmed > 0))
        {
            string upc = CsvCell(r.Upc);
            string qty = CsvCell(r.QtyConfirmed.Value.ToString(CultureInfo.InvariantCulture));
            string price = CsvCell(r.Price.ToString("0.00", CultureInfo.InvariantCulture));
            string unit = "CS";

            sb.Append(upc).Append(',')
              .Append(qty).Append(',')
              .Append(price).Append(',')
              .Append(unit).AppendLine();
        }

        return sb.ToString();
    }

    private static string CsvCell(string? value)
        => $"\"{(value ?? string.Empty).Replace("\"", "\"\"")}\"";

    /// <summary>
    /// Applies the bump-up step logic to QtyInCases.
    ///  <5 -> +1,  <20 -> +5,  <50 -> +10, else 0.
    /// </summary>
    public void BumpQtyInCases()
    {
        int q = AvailableQtyInCases;
        int bump = (q == 0) ? 0
            : (q > 10) ? 10
            : (q > 5) ? 5
            : (q > 3) ? 2
            : (q > 0) ? 1
            : 0;

        if (q > 0)
        {
            ExtraCases = bump;
            QtyInCases = q + bump;
        }
    }

    public void ClearExtras()
    {
        QtyInCases = AvailableQtyInCases;
        ExtraCases = 0;
    }
}
