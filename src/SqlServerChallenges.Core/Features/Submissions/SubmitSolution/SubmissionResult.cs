using SqlServerChallenges.Core.Services;
using SqlServerChallenges.Core.Services.QueryExecutor;

namespace SqlServerChallenges.Core.Features.Submissions.SubmitSolution;

public class SubmissionResult
{
    public bool IsSuccess { get; }
    public bool Passed { get; }
    public IReadOnlyList<SqlSyntaxError>? SyntaxErrors { get; }
    public QueryError? QueryError { get; }

    private SubmissionResult(bool passed)
    {
        IsSuccess = true;
        Passed = passed;
    }

    private SubmissionResult(IReadOnlyList<SqlSyntaxError> syntaxErrors)
    {
        IsSuccess = false;
        Passed = false;
        SyntaxErrors = syntaxErrors;
    }

    public SubmissionResult(QueryError queryError)
    {
        IsSuccess = false;
        Passed = false;
        QueryError = queryError;
    }

    public static SubmissionResult FromComparisionResult(bool passed) => new(passed);
    public static SubmissionResult FromSyntaxError(IReadOnlyList<SqlSyntaxError> errors) => new(errors);
    public static SubmissionResult FromQueryError(QueryError queryError) => new(queryError);
}