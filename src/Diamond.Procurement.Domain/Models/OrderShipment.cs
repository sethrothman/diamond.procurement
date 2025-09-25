namespace Diamond.Procurement.Data.Models;

public sealed class OrderVendorShipment
{
    public int OrderVendorShipmentId { get; set; }
    public int OrderVendorId { get; set; }
    public int TruckNum { get; set; }
    public DateTime ShipmentDate { get; set; }
    public DateTime EstimatedDeliveryDate { get; set; }
}

public sealed class OrderVendorShipmentDetail
{
    public int OrderVendorShipmentDetailId { get; set; }
    public int OrderVendorShipmentId { get; set; }
    public int UpcId { get; set; }
    public int Quantity { get; set; }
    public DateTime InsertDate { get; set; }
}

public sealed class ShipmentListItem
{
    public int OrderVendorShipmentId { get; set; }
    public int TruckNum { get; set; }
    public DateTime ShipmentDate { get; set; }
    public DateTime EstimatedDeliveryDate { get; set; }
    public int LineCount { get; set; }
    public int UnitsInShipment { get; set; }
    public string? InvoiceNum { get; set; }
}

public sealed class ShipmentDetailRow
{
    public int OrderVendorShipmentDetailId { get; set; }
    public int OrderVendorShipmentId { get; set; }
    public int UpcId { get; set; }
    public string Upc { get; set; } = "";
    public string Description { get; set; } = "";
    public int Quantity { get; set; }
    public DateTime ShipmentDate { get; set; }
    public int TruckNum { get; set; }
}

public sealed class OrderVsShipStatusRow
{
    public int UpcId { get; set; }
    public string Upc { get; set; } = "";
    public string Description { get; set; } = "";
    public int OrderedQty { get; set; }
    public int ShippedQty { get; set; }
    public int RemainingQty => Math.Max(0, OrderedQty - ShippedQty);
}

public sealed class ShipmentPivotRow
{
    // Flat rows that the PivotGrid will pivot on:
    public int UpcId { get; set; }
    public string Upc { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime ShipmentDate { get; set; }   // ==> Pivot column
    public int Quantity { get; set; }            // ==> Pivot value
}

public sealed class UpcResolution
{
    public string Input { get; set; } = "";     // original string passed in
    public string Canonical { get; set; } = ""; // normalized (e.g., “00” + 11)
    public int? UpcId { get; set; }             // null if unknown
}

public sealed class MatrixBaseRow
{
    public int UpcId { get; set; }
    public string UPC { get; set; } = "";
    public string? Description { get; set; }
    public int QtyInCases { get; set; }
    public decimal Price { get; set; }
    public string? InvoiceNum { get; set; }
}

public sealed class ShipmentQuantityRow
{
    public int OrderVendorShipmentId { get; set; }
    public int UpcId { get; set; }
    public int Quantity { get; set; }
}

