
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;

namespace SqlServerChallenges.Core.Services.QueryExecutor;

public class QueryExecutorDispatcher
{
    private readonly Dictionary<DatabaseProvider, IQueryExecutor> _executors;

    public QueryExecutorDispatcher(IEnumerable<IQueryExecutor> queryExecutors)
    {
        _executors = queryExecutors.ToDictionary(x => x.Provider);
    }

    public IQueryExecutor GetQueryExecutor(DatabaseProvider provider)
    {
        if (!_executors.TryGetValue(provider, out var executor))
            throw new InvalidOperationException("Provider not found");

        return executor;
    }
}