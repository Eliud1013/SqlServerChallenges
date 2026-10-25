using System.Data;
using System.Data.Common;

namespace SqlServerChallenges.Core.Services.QueryExecutor;

public sealed class QueryReaderResult : IAsyncDisposable
{
    public bool IsSuccess { get; }
    private readonly DbConnection? _connection;
    private readonly DbCommand? _dbCommand;
    private readonly DbDataReader? _dataReader;
    private readonly QueryError? _queryError;

    public QueryReaderResult(
        DbConnection connection,
        DbCommand dbCommand,
        DbDataReader dataReader)
    {
        IsSuccess = true;
        _connection = connection;
        _dbCommand = dbCommand;
        _dataReader = dataReader;
    }

    private QueryReaderResult(QueryErrorType errorType, string message)
    {
        IsSuccess = false;
        _queryError = new QueryError(errorType, message);
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
        if (_dataReader != null) await _dataReader.DisposeAsync();
        if (_dbCommand != null) await _dbCommand.DisposeAsync();
        if (_connection?.State == ConnectionState.Open) await _connection.CloseAsync();
    }
}