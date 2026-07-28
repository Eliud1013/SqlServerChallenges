namespace SqlServerChallenges.Core.Services.SqlExecutor;

public interface IQueryExecutor
{
    DatabaseProvider Provider { get; }
    Task<QueryExecutorResult> ExecuteQueryAsync(string query, int? rowLimit = 50, CancellationToken ct = default);
}