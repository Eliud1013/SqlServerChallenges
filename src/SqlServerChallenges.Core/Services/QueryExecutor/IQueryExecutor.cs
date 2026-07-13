using System.Data;

namespace SqlServerChallenges.Core.Services.SqlExecutor;

public interface IQueryExecutor
{
    DatabaseProvider Provider { get; }
    Task<QueryExecutorResult> ExecuteQueryAsync(string query, CancellationToken ct = default);
}