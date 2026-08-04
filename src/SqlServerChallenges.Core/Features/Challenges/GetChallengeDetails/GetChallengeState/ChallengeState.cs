namespace SqlServerChallenges.Core.Features.Challenges.GetChallengeDetails.GetChallengeState;

public sealed record ChallengeState(
    int CommentCount,
    bool IsSolved);
