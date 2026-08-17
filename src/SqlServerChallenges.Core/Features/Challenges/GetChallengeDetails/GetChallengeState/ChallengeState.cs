namespace SqlServerChallenges.Core.Features.Challenges.GetChallengeDetails.GetChallengeState;

public sealed record ChallengeState(
    int CommentCount,
    int UpVotes,
    bool IsUpVoted,
    bool IsDownVoted,
    bool IsSolved);