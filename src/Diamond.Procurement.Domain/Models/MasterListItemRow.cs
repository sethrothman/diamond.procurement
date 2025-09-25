public sealed class MasterListItemRow
{
    public int MasterListDetailId { get; set; }   // for future inline ops
    public int MasterListId { get; set; }
    public int UpcId { get; set; }
    public string Upc { get; set; } = "";
    public string Description { get; set; } = "";
    public bool IsActive { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime? DateRemoved { get; set; }
    public bool HasAlternateBuyer { get; set; }
}