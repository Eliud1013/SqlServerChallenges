using SqlServerChallenges.Core.Common.Results;
using SqlServerChallenges.Core.Services;

namespace SqlServerChallenges.Core.Features.Submissions;

public static class SubmissionErrors // TODO: Use proper user-friendly error messages
{
    public static readonly Error ChallengeNotFound = new("Submission.ChallengeNotFound", "The challenge was not found.");
    public static readonly Error SolutionNotFound = new("Submission.SolutionNotFound", "Solution not found");
    public static readonly Error UnknownError = new("Submission.UnknownError", "Unknown error");
    
    public static Error<IReadOnlyList<SqlSyntaxError>> SyntaxError(IReadOnlyList<SqlSyntaxError> errors)
        => new("Submission.InvalidSyntax", "Submitted code has syntax errors. ", errors);
}