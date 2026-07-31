using System.Data.Common;
using Microsoft.Data.SqlClient;
using SqlServerChallenges.Core.Services.SqlExecutor;

namespace SqlServerChallenges.Core.Services.QueryReader;

public class MsSqlQueryReader : IQueryReader
{
    public DatabaseProvider Provider => DatabaseProvider.SqlServer;

    private readonly SqlConnection _connection;

    public MsSqlQueryReader(SqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<DbDataReader> ExecuteReaderAsync(string query, CancellationToken ct = default)
    {
        await _connection.OpenAsync(ct);
        var command = new SqlCommand(query, _connection);
        command.CommandTimeout = 6;

        return await command.ExecuteReaderAsync(ct);
    }
}
