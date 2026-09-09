using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;

namespace SqlServerChallenges.Core.Services.TableReferenceExtractor;

public interface ITableReferencesExtractor
{
    DatabaseProvider Provider { get; }
    IReadOnlyDictionary<string, List<string>> Extract(string sql);
}