using Dapper;
using Diamond.Procurement.Data.Contracts;
using Diamond.Procurement.Domain.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;

namespace Diamond.Procurement.Data;

public interface IMasterListRepository
{
    Task<IReadOnlyList<MasterListSummary>> GetAllMasterLists(CancellationToken ct);
    Task<IReadOnlyList<MasterListItemRow>> GetDetailsAsync(int masterListId, CancellationToken ct);

    // Bulk apply: add & remove (strings are raw UPCs; SQL resolves to UpcId)
    Task BulkApplyAsync(int masterListId, IEnumerable<string> upcsToAdd, IEnumerable<string> upcsToRemove, CancellationToken ct);
    Task MarkAsAlternateBuyerPerRowAsync(IEnumerable<AlternateBuyerRow> rows, CancellationToken ct);
}

public sealed class MasterListRepository : IMasterListRepository
{
    private readonly IDbFactory _dbf;
    public MasterListRepository(IDbFactory dbf) => _dbf = dbf;

    /// <summary>
    /// Bulk upsert via TVP. 
    /// - Rows with MasterListId = NULL/0 => INSERT
    /// - Rows with MasterListId > 0      => UPDATE by PK
    /// </summary>
    public async Task LoadAsync(IEnumerable<MasterListRow> rows, CancellationToken ct)
    {
        using var db = _dbf.Create();
        var tvp = BuildTvp(rows);
        var p = new DynamicParameters();
        p.Add("@Rows", tvp.AsTableValuedParameter("dbo.MasterListType"));

        // If you care about counts, you can QuerySingle instead of Execute
        await db.ExecuteAsync(
            new CommandDefinition(
                "dbo.MasterList_Load",
                parameters: p,
                commandType: CommandType.StoredProcedure,
                cancellationToken: ct));
    }

    /// <summary>
    /// Fetch a single row by PK (or null if not found).
    /// </summary>
    public async Task<MasterListRow?> GetByIdAsync(int masterListId, CancellationToken ct)
    {
        using var db = _dbf.Create();
        var p = new { MasterListId = masterListId };

        return await db.QuerySingleOrDefaultAsync<MasterListRow>(
            new CommandDefinition(
                "dbo.MasterList_GetById",
                parameters: p,
                commandType: CommandType.StoredProcedure,
                cancellationToken: ct));
    }

    private static DataTable BuildTvp(IEnumerable<MasterListRow> rows)
    {
        var dt = new DataTable();
        dt.Columns.Add("MasterListId", typeof(int));      // allow DBNull for inserts
        dt.Columns.Add("Name", typeof(string));
        dt.Columns.Add("ListTypeId", typeof(int));
        dt.Columns.Add("BuyerId", typeof(int));
        dt.Columns.Add("VendorId", typeof(int));

        foreach (var r in rows)
        {
            var dr = dt.NewRow();

            // MasterListId: NULL or 0 => treat as insert
            if (r.MasterListId.HasValue && r.MasterListId.Value > 0)
                dr["MasterListId"] = r.MasterListId.Value;
            else
                dr["MasterListId"] = DBNull.Value;

            dr["Name"] = (r.Name ?? string.Empty).Trim();   // normalize
            dr["ListTypeId"] = r.ListTypeId;
            dr["BuyerId"] = r.BuyerId;
            dr["VendorId"] = r.VendorId;

            dt.Rows.Add(dr);
        }

        return dt;
    }

    public async Task<IReadOnlyList<MasterListSummary>> GetAllMasterLists(CancellationToken ct)
    {
        const string sql = @"
            SELECT ml.MasterListId,
                   ml.Name,
                   v.Name AS VendorName,
                   b.BuyerName,
                   ml.ListTypeId

            FROM dbo.MasterList ml
            INNER JOIN dbo.Vendor AS v 
                ON v.VendorId = ml.VendorId
            INNER JOIN dbo.Buyer b 
                ON b.BuyerId  = ml.BuyerId
            ORDER BY ml.Name;
            ";

        using var db = _dbf.Create();
        var rows = await db.QueryAsync<MasterListSummary>(new CommandDefinition(sql, cancellationToken: ct));

        return rows.ToList();
    }

    public async Task<IReadOnlyList<MasterListItemRow>> GetDetailsAsync(int masterListId, CancellationToken ct)
    {
        using var db = _dbf.Create();
        var sql = @"
            SELECT mld.MasterListDetailId, mld.MasterListId, mld.UpcId, u.Upc, u.Description,
                   mld.IsActive, mld.DateAdded, mld.DateRemoved, mld.HasAlternateBuyer
            FROM dbo.MasterListDetail mld
            JOIN dbo.Upc u ON u.UpcId = mld.UpcId
            WHERE mld.MasterListId = @MasterListId
            ORDER BY u.Upc;";
        var rows = await db.QueryAsync<MasterListItemRow>(new CommandDefinition(sql, new { MasterListId = masterListId }, cancellationToken: ct));

        return rows.AsList();
    }

    public async Task BulkApplyAsync(int masterListId, IEnumerable<string> upcsToAdd, IEnumerable<string> upcsToRemove, CancellationToken ct)
    {
        using var db = _dbf.Create();
        var tvpAdd = UpcListTvp(upcsToAdd);
        var tvpRemove = UpcListTvp(upcsToRemove);

        var p = new DynamicParameters();
        p.Add("@MasterListId", masterListId);
        p.Add("@UpcsToAdd", tvpAdd.AsTableValuedParameter("dbo.UpcList"));
        p.Add("@UpcsToRemove", tvpRemove.AsTableValuedParameter("dbo.UpcList"));

        await db.ExecuteAsync(new CommandDefinition("dbo.MasterListDetail_BulkApply", p, commandType: CommandType.StoredProcedure, cancellationToken: ct));
    }

    private static DataTable UpcListTvp(IEnumerable<string> upcs)
    {
        var dt = new DataTable();
        dt.Columns.Add("Upc", typeof(string));
        foreach (var raw in upcs ?? Array.Empty<string>())
        {
            var s = (raw ?? "").Trim();
            if (!string.IsNullOrEmpty(s))
                dt.Rows.Add(s);
        }
        return dt;
    }

    public async Task MarkAsAlternateBuyerPerRowAsync(IEnumerable<AlternateBuyerRow> rows, CancellationToken ct)
    {
        using var db = _dbf.Create();

        var tvp = new DataTable();
        tvp.Columns.Add("MasterListDetailId", typeof(int));
        tvp.Columns.Add("HasAlternateBuyer", typeof(bool)); // per-row value is used

        foreach (var r in rows)
            tvp.Rows.Add(r.MasterListDetailId, r.HasAlternateBuyer);

        var p = new DynamicParameters();
        p.Add("@Rows", tvp.AsTableValuedParameter("dbo.MasterListDetailAlternateBuyerType"));
        p.Add("@HasAlternateBuyer", dbType: DbType.Boolean, value: null); // NULL => use per-row values

        await db.ExecuteAsync("dbo.MasterListDetail_BulkUpdateHasAlternateBuyer", p, commandType: CommandType.StoredProcedure);
    }
}
