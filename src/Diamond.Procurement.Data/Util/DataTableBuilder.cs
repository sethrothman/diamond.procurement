using System;
using System.Collections.Generic;
using System.Data;

namespace Diamond.Procurement.Data;

public sealed class DataTableBuilder<TRow>
{
    private readonly string? _tableName;
    private readonly List<Column> _columns = new();

    public DataTableBuilder(string? tableName = null)
    {
        _tableName = tableName;
    }

    public DataTableBuilder<TRow> AddColumn<TColumn>(string name, Func<TRow, TColumn> valueSelector)
    {
        ArgumentNullException.ThrowIfNull(valueSelector);
        return AddColumn(name, typeof(TColumn), row => valueSelector(row));
    }

    public DataTableBuilder<TRow> AddColumn(string name, Type type, Func<TRow, object?> valueSelector)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Column name cannot be null or whitespace.", nameof(name));
        }

        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(valueSelector);

        var columnType = Nullable.GetUnderlyingType(type) ?? type;
        _columns.Add(new Column(name, columnType, valueSelector));
        return this;
    }

    public DataTable Build(IEnumerable<TRow>? rows)
    {
        var table = string.IsNullOrEmpty(_tableName) ? new DataTable() : new DataTable(_tableName);

        foreach (var column in _columns)
        {
            table.Columns.Add(column.Name, column.Type);
        }

        if (rows is null)
        {
            return table;
        }

        foreach (var item in rows)
        {
            var dataRow = table.NewRow();
            foreach (var column in _columns)
            {
                var value = column.ValueSelector(item);
                dataRow[column.Name] = value ?? DBNull.Value;
            }

            table.Rows.Add(dataRow);
        }

        return table;
    }

    private sealed record Column(string Name, Type Type, Func<TRow, object?> ValueSelector);
}
