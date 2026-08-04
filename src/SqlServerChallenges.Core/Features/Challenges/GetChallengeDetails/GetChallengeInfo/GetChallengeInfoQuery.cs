using SqlServerChallenges.Core.Common.Cache;
using SqlServerChallenges.Core.Common.CQRS.Query;

namespace SqlServerChallenges.Core.Features.Challenges.GetChallengeDetails.GetChallengeInfo;

public class GetChallengeInfoQuery(string Slug) : ICacheableQuery<ChallengeInfo>
{
    public string CacheKey { get; } = CacheKeys.Challenges.Info(Slug);
    public TimeSpan? SlidingExpiration { get; }
    public DateTimeOffset? AbsoluteExpiration { get; } = DateTimeOffset.UtcNow.AddHours(4);
}