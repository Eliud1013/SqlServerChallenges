using SqlServerChallenges.Core.Common.CQRS.Query;

namespace SqlServerChallenges.Core.Features.Challenges.GetChallengeDetails.GetChallengeState;

public sealed record GetChallengeStateQuery(string Slug) : IQuery<ChallengeState>;
