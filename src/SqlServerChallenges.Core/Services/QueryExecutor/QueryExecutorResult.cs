using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace SqlServerChallenges.Core.Services.SqlExecutor;

public sealed record QueryExecutorResult
{
    public bool IsSuccess { get; }
   
    private DataTable? _table { get; }
    private QueryErrorType? _errorType { get; }

    private QueryExecutorResult(DataTable table)
    {
        IsSuccess = true;
        _table = table;
    }

    private QueryExecutorResult(QueryErrorType errorType)
    {
        IsSuccess = false;
        _errorType = errorType;
    }

    public DataTable Table => _table
                              ?? throw new InvalidOperationException("The result does not contain a table.");

    public QueryErrorType ErrorType => _errorType
                                       ?? throw new InvalidOperationException("The result does not contain an error.");

    public static implicit operator QueryExecutorResult(DataTable table) => new(table);
    public static implicit operator QueryExecutorResult(QueryErrorType errorType) => new(errorType);
}