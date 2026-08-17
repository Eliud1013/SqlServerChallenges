using SqlServerChallenges.Core.Data.Entities.Challenges;
using SqlServerChallenges.Core.Services.QueryExecutor;

namespace SqlServerChallenges.Core.Features.Challenges.GetChallengeDetails.GetChallengeInfo;

public record ChallengeInfo(
    Guid ChallengeId,
    string Title,
    int Number,
    string Description,
    ChallengeDifficulty Difficulty,
    string Category,
    OutputTable SampleOutput);