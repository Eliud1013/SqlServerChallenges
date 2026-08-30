using System.Data;
using System.Xml.Linq;
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

    public async Task<QueryExecutorResult> ExecuteQueryAsync(
        string query,
        int? rowLimit = null,
        bool includePlan = false,
        CancellationToken ct = default)
    {
        try
        {
            await _connection.OpenAsync(ct);

            if (includePlan) await ExecuteNonQueryAsync("SET STATISTICS XML ON", ct);

            await using var command = new SqlCommand(query, _connection);
            command.CommandTimeout = 6;

            await using (var reader = await command.ExecuteReaderAsync(ct))
            {
                var table = new DataTable();
                int columnCount = 1;

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);
                    var columnType = reader.GetFieldType(i) ?? typeof(string);

                    if (table.Columns.Contains(columnName))
                    {
                        table.Columns.Add($"{columnName}_{columnCount++}", columnType);
                        continue;
                    }

                    table.Columns.Add(columnName, columnType);
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

                QueryExecutorResult result = table;

                if (includePlan && result.IsSuccess)
                {
                    await reader.NextResultAsync(ct);
                    await reader.ReadAsync(ct);
                    var plan = reader.GetString(0);
                    result.OutputTable.WithPlan(plan);
                }

                return result;
            }
        }
        catch (SqlException ex)
        {
            return ex.Number switch
            {
                -2 => new QueryError(QueryErrorType.QueryTimeout, "The query took too long to execute."),
                207 => new QueryError(QueryErrorType.InvalidColumn, ex.Message),
                229 or 3701 => new QueryError(QueryErrorType.PermissionDenied,
                    "You do not have permission to execute this query."),
                2812 => new QueryError(QueryErrorType.InvalidQuery,
                    "The query contains an invalid statement or calls a function/procedure that does not exist."),
                _ => new QueryError(QueryErrorType.Unknown, "An error occurred while executing the query."),
            };
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "An exception occurred while running user query");
            return new QueryError(QueryErrorType.Unknown, "An error occurred while executing your query");
        }
        finally
        {
            if (includePlan)
                await ExecuteNonQueryAsync("SET STATISTICS XML OFF", ct);

            await _connection.CloseAsync();
        }
    }

    private async Task ExecuteNonQueryAsync(string sql, CancellationToken ct)
    {
        await using var command = new SqlCommand(sql, _connection);
        command.CommandTimeout = 6;
        await command.ExecuteNonQueryAsync(ct);
    }
}