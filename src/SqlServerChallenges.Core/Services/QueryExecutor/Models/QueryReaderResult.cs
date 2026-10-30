using System.Data;
using System.Data.Common;

namespace SqlServerChallenges.Core.Services.QueryExecutor;

public sealed class QueryReaderResult : IAsyncDisposable
{
    public bool IsSuccess { get; }
    private readonly DbConnection? _connection;
    private readonly DbCommand? _command;
    private readonly DbDataReader? _dataReader;
    private readonly Func<ValueTask>? _planCleanupFunc;
    private readonly Func<string, QueryPlanInfo>? _planParseFunc;
    private readonly QueryError? _queryError;
    private bool _disposed;

    public QueryReaderResult(
        DbConnection connection,
        DbCommand command,
        DbDataReader dataReader,
        Func<ValueTask>? planCleanupFunc,
        Func<string, QueryPlanInfo>? planParseFunc)
    {
        IsSuccess = true;
        _connection = connection;
        _command = command;
        _dataReader = dataReader;
        _planCleanupFunc = planCleanupFunc;
        _planParseFunc = planParseFunc;
    }

    private QueryReaderResult(QueryErrorType errorType, string message)
    {
        IsSuccess = false;
        _queryError = new QueryError(errorType, message);
    }

    public async Task<QueryPlanInfo> GetExecutionPlanAsync(CancellationToken ct)
    {
        if (_dataReader is null || _planParseFunc is null)
            throw new InvalidOperationException();

        await _dataReader.NextResultAsync(ct);
        await _dataReader.ReadAsync(ct);
        
        var planXml = _dataReader.GetString(0);
        return _planParseFunc(planXml);
    }

    public DbDataReader DataReader =>
        _dataReader ?? throw new InvalidOperationException("The result does not contain a data reader");

    public QueryError QueryError =>
        _queryError ?? throw new InvalidOperationException("The result does not contain an error.");

    public QueryErrorType ErrorType =>
        _queryError?.Type ?? throw new InvalidOperationException("The result does not contain an error.");

    public string ErrorMessage =>
        _queryError?.Message ?? throw new InvalidOperationException("The result does not contain an error.");

    public static implicit operator QueryReaderResult(QueryError error) => new(error.Type, error.Message);

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;
        
        try
        {
            if (_dataReader is not null)
                await _dataReader.DisposeAsync();
        }
        finally
        {
            try
            {
                if (_command is not null)
                    await _command.DisposeAsync();
            }
            finally
            {
                try
                {
                    if (_planCleanupFunc is not null)
                        await _planCleanupFunc();
                }
                finally
                {
                    if (_connection?.State == ConnectionState.Open)
                        await _connection.CloseAsync();
                }
            }
        }
    }
}
