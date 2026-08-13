namespace SqlServerChallenges.Core.Data.Entities.ChallengeVote;

public class ChallengeVote
{
    public required Guid ChallengeId { get; init; }
    public required string UserId { get; init; }
    public required VoteType Type { get; set; }
    public required DateTime VotedAtUtc { get; set; }
}