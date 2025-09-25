using System;
using System.Data;
using System.Linq;
using Dapper;
using Diamond.Procurement.Data.Contracts;
using Diamond.Procurement.Domain.Models;
using DocumentFormat.OpenXml.Spreadsheet;

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
    private static readonly DataTableBuilder<MasterListRow> MasterListTvpBuilder =
        new DataTableBuilder<MasterListRow>()
            .AddColumn("MasterListId", r => r.MasterListId.HasValue && r.MasterListId.Value > 0 ? r.MasterListId : null)
            .AddColumn("Name", r => (r.Name ?? string.Empty).Trim())
            .AddColumn("ListTypeId", r => r.ListTypeId)
            .AddColumn("BuyerId", r => r.BuyerId)
            .AddColumn("VendorId", r => r.VendorId);

    private static readonly DataTableBuilder<string> UpcListTvpBuilder =
        new DataTableBuilder<string>()
            .AddColumn("Upc", s => s);

    private static readonly DataTableBuilder<AlternateBuyerRow> AlternateBuyerTvpBuilder =
        new DataTableBuilder<AlternateBuyerRow>()
            .AddColumn("MasterListDetailId", r => r.MasterListDetailId)
            .AddColumn("HasAlternateBuyer", r => r.HasAlternateBuyer);

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

    private static DataTable BuildTvp(IEnumerable<MasterListRow> rows) =>
        MasterListTvpBuilder.Build(rows ?? Array.Empty<MasterListRow>());

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
        var normalized = (upcs ?? Array.Empty<string>())
            .Select(s => (s ?? string.Empty).Trim())
            .Where(s => !string.IsNullOrEmpty(s));

        return UpcListTvpBuilder.Build(normalized);
    }

    public async Task MarkAsAlternateBuyerPerRowAsync(IEnumerable<AlternateBuyerRow> rows, CancellationToken ct)
    {
        using var db = _dbf.Create();

        using var tvp = AlternateBuyerTvpBuilder.Build(rows ?? Array.Empty<AlternateBuyerRow>());

        var p = new DynamicParameters();
        p.Add("@Rows", tvp.AsTableValuedParameter("dbo.MasterListDetailAlternateBuyerType"));
        p.Add("@HasAlternateBuyer", dbType: DbType.Boolean, value: null); // NULL => use per-row values

        await db.ExecuteAsync("dbo.MasterListDetail_BulkUpdateHasAlternateBuyer", p, commandType: CommandType.StoredProcedure);
    }
}
