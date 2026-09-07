using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;

namespace SqlServerChallenges.Core.Services.TableReferenceExtractor;

public class TableReferencesExtractorDispatcher
{
    private readonly Dictionary<DatabaseProvider, ITableReferencesExtractor> _extractors;

    public TableReferencesExtractorDispatcher(IEnumerable<ITableReferencesExtractor> extractors)
    {
        _extractors = extractors.ToDictionary(x => x.Provider);
    }

    public IDictionary<string, List<string>> Extract(string sql, DatabaseProvider provider)
    {
        if (!_extractors.TryGetValue(provider, out var extractor))
            throw new InvalidOperationException();

        return extractor.Extract(sql);
    }
}