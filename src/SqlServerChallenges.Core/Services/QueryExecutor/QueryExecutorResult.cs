using System.Data;

namespace SqlServerChallenges.Core.Services.SqlExecutor;

public sealed record QueryExecutorResult
{
    public bool IsSuccess { get; }

    private readonly IReadOnlyList<string>? _columns;
    private readonly IReadOnlyList<IReadOnlyDictionary<string, object?>>? _rows;
    private readonly QueryErrorType? _errorType;

    private QueryExecutorResult(IReadOnlyList<string> columns, IReadOnlyList<IReadOnlyDictionary<string, object?>> rows)
    {
        IsSuccess = true;
        _columns = columns;
        _rows = rows;
    }

    private QueryExecutorResult(QueryErrorType errorType)
    {
        IsSuccess = false;
        _errorType = errorType;
    }

    public IReadOnlyList<string> Columns => _columns
        ?? throw new InvalidOperationException("The result does not contain a query result.");

    public IReadOnlyList<IReadOnlyDictionary<string, object?>> Rows => _rows
        ?? throw new InvalidOperationException("The result does not contain a query result.");

    public QueryErrorType ErrorType => _errorType
        ?? throw new InvalidOperationException("The result does not contain an error.");

    public static implicit operator QueryExecutorResult(DataTable table)
    {
        var columns = Enumerable.Range(0, table.Columns.Count)
            .Select(i => table.Columns[i].ColumnName)
            .ToList();

        var rows = table.AsEnumerable()
            .Select(r => (IReadOnlyDictionary<string, object?>)columns
                .ToDictionary(c => c, c => r[c] is DBNull ? null : r[c]))
            .ToList();

        return new QueryExecutorResult(columns, rows);
    }

    public static implicit operator QueryExecutorResult(QueryErrorType errorType) => new(errorType);
}
