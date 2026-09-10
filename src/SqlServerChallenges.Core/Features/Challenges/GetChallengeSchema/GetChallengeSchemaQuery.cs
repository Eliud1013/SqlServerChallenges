using SqlServerChallenges.Core.Common.Cache;
using SqlServerChallenges.Core.Common.CQRS.Query;
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;
using SqlServerChallenges.Core.Services.SchemaReader;

namespace SqlServerChallenges.Core.Features.Challenges.GetChallengeSchema;

public record GetChallengeSchemaQuery(Guid ChallengeId, DatabaseProvider Provider)
    : ICacheableQuery<IReadOnlyDictionary<string, IReadOnlyList<ColumnInfo>>>
{
    public string CacheKey => CacheKeys.Challenges.Schema(ChallengeId, Provider);
    public TimeSpan? SlidingExpiration => null;
    public DateTimeOffset? AbsoluteExpiration => null;
}