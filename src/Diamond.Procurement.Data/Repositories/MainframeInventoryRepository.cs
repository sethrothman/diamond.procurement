using System.Data;
using Dapper;
using Diamond.Procurement.Domain.Models;

namespace Diamond.Procurement.Data;

public sealed class MainframeInventoryRepository
{
    private readonly IDbFactory _dbf;
    public MainframeInventoryRepository(IDbFactory dbf) => _dbf = dbf;

    public async Task LoadAsync(IEnumerable<MainframeInventoryRow> rows, CancellationToken ct)
    {
        using var db = _dbf.Create();
        var tvp = BuildTvp(rows);
        var p = new DynamicParameters();
        p.Add("@Rows", tvp.AsTableValuedParameter("dbo.MainframeInventoryType"));
        await db.ExecuteAsync(new CommandDefinition(
            "dbo.MainframeInventory_Load",
            p,
            commandType: CommandType.StoredProcedure,
            cancellationToken: ct,
            commandTimeout: 300));
    }

    private static DataTable BuildTvp(IEnumerable<MainframeInventoryRow> rows)
    {
        var dt = new DataTable();
        dt.Columns.Add("Upc", typeof(string));
        dt.Columns.Add("Description", typeof(string));
        dt.Columns.Add("CasePack", typeof(int));
        dt.Columns.Add("QtyAvailable", typeof(int));
        dt.Columns.Add("QtyOnPo", typeof(int));
        dt.Columns.Add("QtyOverstock", typeof(int));
        dt.Columns.Add("ListPrice", typeof(decimal));   // NEW: carry through to proc
        dt.Columns.Add("EffectiveDate", typeof(DateTime));
        dt.Columns.Add("HI", typeof(int));
        dt.Columns.Add("TI", typeof(int));

        foreach (var r in rows)
        {
            var row = dt.NewRow();
            row["Upc"] = r.Upc;
            row["Description"] = (r.Description ?? string.Empty).Trim();
            row["CasePack"] = r.CasePack.HasValue ? r.CasePack.Value : DBNull.Value;
            row["QtyAvailable"] = r.QtyAvailable;
            row["QtyOnPo"] = r.QtyOnPo;
            row["QtyOverstock"] = r.QtyOverstock;
            row["ListPrice"] = r.ListPrice.HasValue ? r.ListPrice.Value : DBNull.Value; // NEW
            row["EffectiveDate"] = r.EffectiveDate.ToDateTime(TimeOnly.MinValue);
            row["HI"] = r.HI;
            row["TI"] = r.TI;
            dt.Rows.Add(row);
        }
        return dt;
    }
}
