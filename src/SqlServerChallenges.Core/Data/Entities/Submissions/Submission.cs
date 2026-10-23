namespace SqlServerChallenges.Core.Data.Entities.Submissions;

public class Submission
{
    public Guid Id { get; init; }
    public required Guid ChallengeId { get; init; }
    public required string UserId { get; init; }
    public required bool Passed { get; init; }
    public required DateTime SubmittedAtUtc { get; init; }
}