using SqlServerChallenges.Core.Data.Entities.Categories;
using SqlServerChallenges.Core.Data.Entities.Challenges;

namespace SqlServerChallenges.Core.Features.Challenges.ListChallenges;

public sealed record ChallengeEntry(
    Guid Id,
    string Title,
    string Category,
    ChallengeDifficulty Difficulty,
    int Acceptance,
    bool Solved);