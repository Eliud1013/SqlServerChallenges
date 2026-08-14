using SqlServerChallenges.Core.Common.Cache;
using SqlServerChallenges.Core.Common.CQRS.Query;

namespace SqlServerChallenges.Core.Features.Challenges.GetChallengeDetails.GetChallengeInfo;

public class GetChallengeInfoQuery(Guid challengeId, string slug) : ICacheableQuery<ChallengeInfo>
{
    public Guid ChallengeId { get; } = challengeId;
    public string CacheKey { get; } = CacheKeys.Challenges.Info(slug);
    public TimeSpan? SlidingExpiration { get; }
    public DateTimeOffset? AbsoluteExpiration { get; } = DateTimeOffset.UtcNow.AddHours(4);
}