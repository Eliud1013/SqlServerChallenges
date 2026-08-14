using SqlServerChallenges.Core.Common.CQRS.Query;

namespace SqlServerChallenges.Core.Features.Challenges.GetChallengeDetails.GetChallengeState;

public sealed record GetChallengeStateQuery(Guid ChallengeId) : IQuery<ChallengeState>;
