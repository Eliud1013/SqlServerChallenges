using SqlServerChallenges.Core.Common.CQRS.Query;
using SqlServerChallenges.Core.Data.Entities.Challenges;

namespace SqlServerChallenges.Core.Features.Challenges.ListChallenges;

public sealed record ListChallengesQuery(string? Title, string? CategoryName, ChallengeDifficulty? Difficulty)
    : IQuery<IReadOnlyList<ChallengeEntry>>;

public sealed record ChallengeEntry(
    Guid Id,
    string Title,
    string TaskDescription,
    ChallengeDifficulty Difficulty,
    IReadOnlyCollection<string> Categories);