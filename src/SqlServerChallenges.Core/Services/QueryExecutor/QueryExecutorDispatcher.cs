namespace SqlServerChallenges.Core.Services.SqlExecutor;

public class QueryExecutorDispatcher
{
    private readonly Dictionary<DatabaseProvider, IQueryExecutor> _executors;

    public QueryExecutorDispatcher(IEnumerable<IQueryExecutor> queryExecutors)
    {
        _executors = queryExecutors.ToDictionary(x => x.Provider);
    }

    public async Task<QueryExecutorResult> ExecuteQueryAsync(
        string query,
        DatabaseProvider provider,
        int? rowLimit = null,
        CancellationToken ct = default)
    {
        if (!_executors.TryGetValue(provider, out var executor))
            throw new InvalidOperationException("Provider not found");

        return await executor.ExecuteQueryAsync(query, rowLimit, ct: ct);
    }
}