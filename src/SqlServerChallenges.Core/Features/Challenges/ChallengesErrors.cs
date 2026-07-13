using SqlServerChallenges.Core.Common.Results;
using SqlServerChallenges.Core.Services;

namespace SqlServerChallenges.Core.Features.Challenges;

public static class ChallengesErrors
{
    public static Error NotFound = new("Challenge.NotFound", "The challenge was not found.");

    public static Error<IReadOnlyList<SqlSyntaxError>> SyntaxError(IReadOnlyList<SqlSyntaxError> errors)
        => new("Challenge.InvalidSyntax", "Submitted code has syntax errors. ", errors);
}