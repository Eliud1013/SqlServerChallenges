using System.Data;

namespace SqlServerChallenges.Core.Services.QueryExecutor.Models;

public sealed record QueryOutputResult
{
    public bool IsSuccess { get; }
    private readonly QueryOutput? _queryOutput;
    private readonly QueryError? _queryError;

    private QueryOutputResult(IReadOnlyList<string> columns, IReadOnlyList<IReadOnlyDictionary<string, object?>> rows)
    {
        IsSuccess = true;
        var outputTable = new OutputTable(columns, rows);

        _queryOutput = new QueryOutput(outputTable);
    }

    private QueryOutputResult(QueryErrorType errorType, string message)
    {
        IsSuccess = false;
        _queryError = new QueryError(errorType, message);
    }

    public QueryOutput QueryOutput =>
        _queryOutput ?? throw new InvalidOperationException("The result does not contain a query output");

    public OutputTable OutputTable =>
        _queryOutput?.OutputTable ?? throw new InvalidOperationException("The result does not contain a table.");

    public QueryPlanInfo QueryPlan =>
        _queryOutput?.QueryPlan ?? throw new InvalidOperationException("The result does not contain a table.");

    public IReadOnlyList<string> Columns =>
        _queryOutput?.OutputTable.Columns ??
        throw new InvalidOperationException("The result does not contain a table.");

    public IReadOnlyList<IReadOnlyDictionary<string, object?>> Rows =>
        _queryOutput?.OutputTable.Rows ?? throw new InvalidOperationException("The result does not contain a table.");

    public QueryError QueryError =>
        _queryError ?? throw new InvalidOperationException("The result does not contain an error.");

    public QueryErrorType ErrorType =>
        _queryError?.Type ?? throw new InvalidOperationException("The result does not contain an error.");

    public string ErrorMessage =>
        _queryError?.Message ?? throw new InvalidOperationException("The result does not contain an error.");

    public static implicit operator QueryOutputResult(DataTable table)
    {
        var columns = Enumerable.Range(0, table.Columns.Count)
            .Select(i => table.Columns[i].ColumnName)
            .ToList();

        var rows = table.AsEnumerable()
            .Select(r => (IReadOnlyDictionary<string, object?>)columns
                .ToDictionary(c => c, c => r[c] is DBNull ? null : r[c], StringComparer.OrdinalIgnoreCase))
            .ToList();

        return new QueryOutputResult(columns.AsReadOnly(), rows.AsReadOnly());
    }

    public static implicit operator QueryOutputResult(QueryError error) => new(error.Type, error.Message);
}