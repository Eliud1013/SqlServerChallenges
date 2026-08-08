using System.Data;

namespace SqlServerChallenges.Core.Services.QueryExecutor;

public record OutputTable(
    IReadOnlyList<string> Columns,
    IReadOnlyList<IReadOnlyDictionary<string, object?>> Rows);

public sealed record QueryExecutorResult
{
    public bool IsSuccess { get; }
    private readonly OutputTable? _outputTable;
    private readonly QueryErrorType? _errorType;

    private QueryExecutorResult(IReadOnlyList<string> columns, IReadOnlyList<IReadOnlyDictionary<string, object?>> rows)
    {
        IsSuccess = true;
        _outputTable = new(columns, rows);
    }

    private QueryExecutorResult(QueryErrorType errorType)
    {
        IsSuccess = false;
        _errorType = errorType;
    }
    
    public OutputTable OutputTable => _outputTable
        ?? throw new InvalidOperationException("The result does not contain a table.");
    
    public IReadOnlyList<string> Columns => _outputTable?.Columns
        ?? throw new InvalidOperationException("The result does not contain a table."); 
    
    public IReadOnlyList<IReadOnlyDictionary<string, object?>> Rows => _outputTable?.Rows
        ?? throw new InvalidOperationException("The result does not contain a table.");

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
