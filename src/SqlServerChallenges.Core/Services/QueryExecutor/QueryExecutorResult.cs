using System.Data;

namespace SqlServerChallenges.Core.Services.QueryExecutor;

public record QueryError(QueryErrorType Type, string Message);

public record OutputTable
{
    public IReadOnlyList<string> Columns { get;  }
    public IList<IDictionary<string, object?>> Rows { get; private set; }

    public OutputTable(IReadOnlyList<string> columns, IList<IDictionary<string, object?>> rows)
    {
        Columns = columns;
        Rows = rows;
    }

    public void OrderRows()
    {
        var orderBy = Columns.First();
        Rows = Rows.OrderBy(r => r[orderBy]?.ToString() ?? "")
            .ToList();
    }
}

public sealed record QueryExecutorResult
{
    public bool IsSuccess { get; }
    private readonly OutputTable? _outputTable;
    private readonly QueryError? _queryError;

    private QueryExecutorResult(IReadOnlyList<string> columns, IList<IDictionary<string, object?>> rows)
    {
        IsSuccess = true;
        _outputTable = new(columns, rows);
    }

    private QueryExecutorResult(QueryErrorType errorType, string message)
    {
        IsSuccess = false;
        _queryError = new QueryError(errorType, message);
    }

    public OutputTable OutputTable =>
        _outputTable ?? throw new InvalidOperationException("The result does not contain a table.");

    public IReadOnlyList<string> Columns =>
        _outputTable?.Columns ?? throw new InvalidOperationException("The result does not contain a table.");

    public IList<IDictionary<string, object?>> Rows =>
        _outputTable?.Rows ?? throw new InvalidOperationException("The result does not contain a table.");

    public QueryErrorType ErrorType =>
        _queryError?.Type ?? throw new InvalidOperationException("The result does not contain an error.");

    public string ErrorMessage =>
        _queryError?.Message ?? throw new InvalidOperationException("The result does not contain an error.");

    public static implicit operator QueryExecutorResult(DataTable table)
    {
        var columns = Enumerable.Range(0, table.Columns.Count)
            .Select(i => table.Columns[i].ColumnName)
            .ToList();

        var rows = table.AsEnumerable()
            .Select(r => (IDictionary<string, object?>)columns
                .ToDictionary(c => c, c => r[c] is DBNull ? null : r[c]))
            .ToList();

        return new QueryExecutorResult(columns, rows);
    }

    public static implicit operator QueryExecutorResult(QueryError error) => new(error.Type, error.Message);
}