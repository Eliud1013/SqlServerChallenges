using SqlServerChallenges.Core.Common.CQRS;

namespace SqlServerChallenges.Core.Features.Challenges.SubmitSolution;

public sealed record SubmitSolutionCommand(Guid ChallengeId, SubmissionType SubmissionType, string UserQuery) : ICommand;