using SqlServerChallenges.Core.Common.Results;
using SqlServerChallenges.Core.Services;

namespace SqlServerChallenges.Core.Features.Challenges;

public static class ChallengesErrors
{
    public static Error NotFound = new("Challenge.NotFound", "The challenge was not found.");
    public static Error SolutionNotFound = new("Challenge.SolutionNotFound", "Could not find a solution for the challenge");
    
    public static readonly Error UnknownError = new("Challenge.UnknownError", "An unknown error occurred.");
    
    public static Error<IReadOnlyList<SqlSyntaxError>> SyntaxError(IReadOnlyList<SqlSyntaxError> errors)
        => new("Challenge.InvalidSyntax", "Submitted code has syntax errors. ", errors);
}