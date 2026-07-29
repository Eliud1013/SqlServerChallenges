using SqlServerChallenges.Core.Common.Cache;
using SqlServerChallenges.Core.Common.CQRS.Query;
using SqlServerChallenges.Core.Services.SqlExecutor;

namespace SqlServerChallenges.Core.Features.Challenges.GetSolution;

public sealed record GetSolutionSampleQuery(Guid ChallengeId) : ICacheableQuery<SampleOutput>
{
    public string CacheKey { get; }  = CacheKeys.Challenges.SolutionSample(ChallengeId);
    public TimeSpan? SlidingExpiration { get; }
    public DateTimeOffset? AbsoluteExpiration { get; }
}