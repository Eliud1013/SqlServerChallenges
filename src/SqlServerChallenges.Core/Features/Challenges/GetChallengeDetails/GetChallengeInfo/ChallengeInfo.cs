using SqlServerChallenges.Core.Data.Entities.Challenges;

namespace SqlServerChallenges.Core.Features.Challenges.GetChallengeDetails.GetChallengeInfo;

public record ChallengeInfo(
    string Title,
    string Description,
    ChallengeDifficulty Difficulty,
    string Category);