using System.Data;
using Microsoft.Data.SqlClient;
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;

namespace SqlServerChallenges.Core.Services.QueryExecutor;

public class MsSqlQueryExecutor : IQueryExecutor
{
    public DatabaseProvider Provider => DatabaseProvider.SqlServer;

    private readonly SqlConnection _connection;

    public MsSqlQueryExecutor(SqlConnection connection)
    {
        _connection = connection;
    }

    public async Task<QueryExecutorResult> ExecuteQueryAsync(string query, int? rowLimit = 50, CancellationToken ct = default)
    {
        try
        {
            await _connection.OpenAsync(ct);
            await using var command = new SqlCommand(query, _connection);
            command.CommandTimeout = 6;

            await using var reader = await command.ExecuteReaderAsync(ct);
            var table = new DataTable();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                table.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
            }

            int rowNumber = 0;

            while (rowNumber < rowLimit && await reader.ReadAsync(ct))
            {
                var row = table.NewRow();

                for (int i = 0; i < reader.FieldCount; i++)
                    row[i] = reader.IsDBNull(i) ? DBNull.Value : reader.GetValue(i);

                table.Rows.Add(row);
                rowNumber++;
            }

            return table;
        }
        catch (SqlException ex)
        {
            return ex.Number switch
            {
                -2 => QueryErrorType.QueryTimeout,
                229 => QueryErrorType.PermissionDenied,
                _ => QueryErrorType.Unknown
            };
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
}
