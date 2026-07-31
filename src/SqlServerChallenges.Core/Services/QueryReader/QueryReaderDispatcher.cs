using System.Data.Common;
using SqlServerChallenges.Core.Services.SqlExecutor;

namespace SqlServerChallenges.Core.Services.QueryReader;

public class QueryReaderDispatcher
{
    private readonly Dictionary<DatabaseProvider, IQueryReader> _readers;

    public QueryReaderDispatcher(IEnumerable<IQueryReader> readers)
    {
        _readers = readers.ToDictionary(x => x.Provider);
    }

    public async Task<DbDataReader> ExecuteReaderAsync(
        string query,
        DatabaseProvider provider,
        CancellationToken ct = default)
    {
        if (!_readers.TryGetValue(provider, out var reader))
            throw new InvalidOperationException("Provider not found");

        return await reader.ExecuteReaderAsync(query, ct);
    }
}
