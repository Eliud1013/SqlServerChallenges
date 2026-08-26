using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;

namespace SqlServerChallenges.Core.Services.QueryExecutor;

public class MsSqlQueryExecutor : IQueryExecutor
{
    public DatabaseProvider Provider => DatabaseProvider.SqlServer;

    private readonly SqlConnection _connection;
    private readonly ILogger<MsSqlQueryExecutor> _logger;

    public MsSqlQueryExecutor(SqlConnection connection, ILogger<MsSqlQueryExecutor> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task<QueryExecutorResult> ExecuteQueryAsync(string query, int? rowLimit = null,
        CancellationToken ct = default)
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

            while ((rowLimit is null || rowNumber < rowLimit) && await reader.ReadAsync(ct))
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
                -2 => new QueryError(QueryErrorType.QueryTimeout, "The query took too long to execute."),
                207 => new QueryError(QueryErrorType.InvalidColumn, ex.Message),
                229 or 3701 => new QueryError(QueryErrorType.PermissionDenied, "You do not have permission to execute this query."),
                2812 => new QueryError(QueryErrorType.InvalidQuery, "The query contains an invalid statement or calls a function/procedure that does not exist."),
                _ =>  new QueryError(QueryErrorType.Unknown, "An error occurred while executing the query."),
            };
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "An exception ocurred while running user query");
            return new QueryError(QueryErrorType.Unknown, "An error ocurred while executing your query");
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
}