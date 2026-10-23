using SqlServerChallenges.Core.Common.CQRS;
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;

namespace SqlServerChallenges.Core.Features.Submissions.SubmitSolution;

public sealed record SubmitSolutionCommand(
    Guid ChallengeId,
    string UserQuery,
    DatabaseProvider Provider) : ICommand<SubmissionResult>;