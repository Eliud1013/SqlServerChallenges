using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;

namespace SqlServerChallenges.Core.Services.QueryExecutor;

public interface IQueryExecutor
{
    DatabaseProvider Provider { get; }
    Task<QueryExecutorResult> ExecuteQueryAsync(string query, int? rowLimit = 50, CancellationToken ct = default);
}