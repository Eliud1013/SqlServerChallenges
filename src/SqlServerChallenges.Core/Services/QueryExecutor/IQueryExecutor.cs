using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;
using SqlServerChallenges.Core.Services.QueryExecutor.Models;

namespace SqlServerChallenges.Core.Services.QueryExecutor;

public interface IQueryExecutor
{
    DatabaseProvider Provider { get; }

    Task<QueryOutputResult> ExecuteQueryAsync(
        string query,
        int? rowLimit = 50,
        bool includePlan = false,
        CancellationToken ct = default);

    Task<QueryReaderResult> ExecuteReaderAsync(string query, CancellationToken ct = default);
}