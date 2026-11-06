using SqlServerChallenges.Core.Services;
using SqlServerChallenges.Core.Services.QueryExecutor;

namespace SqlServerChallenges.Core.Features.Submissions.SubmitSolution;

public class SubmissionResult
{
    public bool ExecutedSuccessfully { get; }
    public bool Passed { get; }
    public QueryPlanInfo? QueryPlan { get; }
    public IReadOnlyList<SqlSyntaxError>? SyntaxErrors { get; }
    public QueryError? QueryError { get; }

    private SubmissionResult(bool passed, QueryPlanInfo? queryPlan)
    {
        ExecutedSuccessfully = true;
        Passed = passed;
        QueryPlan = queryPlan;
    }

    private SubmissionResult(IReadOnlyList<SqlSyntaxError> syntaxErrors)
    {
        ExecutedSuccessfully = false;
        Passed = false;
        SyntaxErrors = syntaxErrors;
    }

    public SubmissionResult(QueryError queryError)
    {
        ExecutedSuccessfully = false;
        Passed = false;
        QueryError = queryError;
    }

    public static SubmissionResult FromComparisionResult(bool passed, QueryPlanInfo? queryPlan = null)
    {
        bool hasPlan = queryPlan is not null;

        if ((passed && !hasPlan) || (!passed && hasPlan))
            throw new InvalidOperationException();

        return new(passed, queryPlan);
    }


    public static SubmissionResult FromSyntaxError(IReadOnlyList<SqlSyntaxError> errors) => new(errors);
    public static SubmissionResult FromQueryError(QueryError queryError) => new(queryError);
}