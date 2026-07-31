using System.Data;
using System.Data.Common;

namespace SqlServerChallenges.Core.Services.QueryResultComparer;

public class QueryResultComparer
{
    public async Task<QueryComparisonResult> CompareAsync(
        DataTable expected,
        DbDataReader actual,
        bool requiredOrdering,
        CancellationToken ct = default)
    {
        var actualColumns = Enumerable.Range(0, actual.FieldCount)
            .Select(actual.GetName)
            .ToArray();

        if (!ColumnsMatch(expected, actualColumns))
            return new QueryComparisonResult(false, expected.Rows.Count, -1, null);

        return requiredOrdering
            ? await CompareOrderedAsync(expected, actual, ct)
            : await CompareUnorderedAsync(expected, actual, ct);
    }

    private static async Task<QueryComparisonResult> CompareOrderedAsync(
        DataTable expected,
        DbDataReader actual,
        CancellationToken ct)
    {
        int i = 0;

        while (await actual.ReadAsync(ct))
        {
            if (i >= expected.Rows.Count)
                return new QueryComparisonResult(false, expected.Rows.Count, i + 1, i);

            if (!RowEquals(expected.Rows[i], actual))
                return new QueryComparisonResult(false, expected.Rows.Count, i + 1, i);

            i++;
        }

        if (i < expected.Rows.Count)
            return new QueryComparisonResult(false, expected.Rows.Count, i, i);

        return new QueryComparisonResult(true, expected.Rows.Count, i, null);
    }

    private static async Task<QueryComparisonResult> CompareUnorderedAsync(
        DataTable expected,
        DbDataReader actual,
        CancellationToken ct)
    {
        var actualTable = await MaterializeAsync(actual, ct);

        if (actualTable.Rows.Count != expected.Rows.Count)
            return new QueryComparisonResult(false, expected.Rows.Count, actualTable.Rows.Count,
                Math.Min(expected.Rows.Count, actualTable.Rows.Count));

        var expectedSorted = SortTable(expected);
        var actualSorted = SortTable(actualTable);

        for (int i = 0; i < expectedSorted.Rows.Count; i++)
        {
            if (!RowEquals(expectedSorted.Rows[i], actualSorted.Rows[i]))
                return new QueryComparisonResult(false, expectedSorted.Rows.Count, actualSorted.Rows.Count, i);
        }

        return new QueryComparisonResult(true, expectedSorted.Rows.Count, actualSorted.Rows.Count, null);
    }

    private static bool ColumnsMatch(DataTable expected, string[] actualColumns)
    {
        if (expected.Columns.Count != actualColumns.Length)
            return false;

        for (int i = 0; i < actualColumns.Length; i++)
        {
            if (!string.Equals(expected.Columns[i].ColumnName, actualColumns[i], StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }

    private static bool RowEquals(DataRow row, DbDataReader reader)
    {
        for (int c = 0; c < reader.FieldCount; c++)
        {
            var expectedValue = row[c];
            var actualValue = reader.IsDBNull(c) ? DBNull.Value : reader.GetValue(c);

            if (!ValuesEqual(expectedValue, actualValue))
                return false;
        }

        return true;
    }

    private static bool RowEquals(DataRow left, DataRow right)
    {
        for (int c = 0; c < left.Table.Columns.Count; c++)
        {
            if (!ValuesEqual(left[c], right[c]))
                return false;
        }

        return true;
    }

    private static bool ValuesEqual(object? a, object? b)
    {
        bool aNull = a is null or DBNull;
        bool bNull = b is null or DBNull;

        if (aNull || bNull)
            return aNull && bNull;

        return a!.Equals(b);
    }

    private static async Task<DataTable> MaterializeAsync(DbDataReader reader, CancellationToken ct)
    {
        var table = new DataTable();

        for (int c = 0; c < reader.FieldCount; c++)
            table.Columns.Add(reader.GetName(c), reader.GetFieldType(c) ?? typeof(object));

        while (await reader.ReadAsync(ct))
        {
            var row = table.NewRow();

            for (int c = 0; c < reader.FieldCount; c++)
                row[c] = reader.IsDBNull(c) ? DBNull.Value : reader.GetValue(c);

            table.Rows.Add(row);
        }

        return table;
    }

    private static DataTable SortTable(DataTable table)
    {
        var sort = string.Join(", ",
            table.Columns.Cast<DataColumn>().Select(c => $"[{c.ColumnName}] ASC"));

        var view = new DataView(table) { Sort = sort };
        return view.ToTable();
    }
}
