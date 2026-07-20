using SqlServerChallenges.Core.Common.Results;
using SqlServerChallenges.Core.Services;

namespace SqlServerChallenges.Core.Features.Submissions;

public static class SubmissionErrors
{
    public static readonly Error ChallengeNotFound = new("Submission.ChallengeNotFound", "The challenge was not found.");

    public static Error<IReadOnlyList<SqlSyntaxError>> SyntaxError(IReadOnlyList<SqlSyntaxError> errors)
        => new("Submission.InvalidSyntax", "Submitted code has syntax errors. ", errors);
}