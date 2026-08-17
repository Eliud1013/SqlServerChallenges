using SqlServerChallenges.Core.Common.Cache;
using SqlServerChallenges.Core.Common.CQRS.Query;

namespace SqlServerChallenges.Core.Features.Challenges.GetChallengeDetails.GetChallengeInfo;

public class GetChallengeInfoQuery(string slug) : ICacheableQuery<ChallengeInfo>
{
    public string Slug = slug;
    public string CacheKey { get; } = CacheKeys.Challenges.Info(slug);
    public TimeSpan? SlidingExpiration { get; }
    public DateTimeOffset? AbsoluteExpiration { get; } = DateTimeOffset.UtcNow.AddHours(4);
}