using Microsoft.EntityFrameworkCore;
using SqlServerChallenges.Core.Common.CQRS;
using SqlServerChallenges.Core.Common.Results;
using SqlServerChallenges.Core.Data;
using SqlServerChallenges.Core.Services;
using SqlServerChallenges.Core.Services.QueryExecutor;

namespace SqlServerChallenges.Core.Features.Submissions.RunUserSql;

public class RunUserSqlHandler : ICommandHandler<RunUserSqlCommand, RunResult>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly SyntaxCheckerDispatcher _syntaxCheckerDispatcher;
    private readonly QueryExecutorDispatcher _queryExecutorDispatcher;


    public RunUserSqlHandler(
        ApplicationDbContext dbContext,
        SyntaxCheckerDispatcher syntaxCheckerDispatcher,
        QueryExecutorDispatcher queryExecutorDispatcher)
    {
        _dbContext = dbContext;
        _queryExecutorDispatcher = queryExecutorDispatcher;
        _syntaxCheckerDispatcher = syntaxCheckerDispatcher;
    }

    public async Task<Result<RunResult>> Handle(RunUserSqlCommand request,
        CancellationToken cancellationToken)
    {
        int defaultRowLimit = 50;
        
        var challenge = await _dbContext.Challenges
            .FirstOrDefaultAsync(c => c.Id == request.ChallengeId, cancellationToken);

        if (challenge is null)
            return SubmissionErrors.ChallengeNotFound;

        var userQuery = request.UserQuery;
        var provider = request.provider;

        var syntaxValidationResult = _syntaxCheckerDispatcher.Validate(userQuery, provider);

        if (syntaxValidationResult.IsInvalid)
            return SubmissionErrors.SyntaxError(syntaxValidationResult.Errors);

        var queryResult =
            await _queryExecutorDispatcher.ExecuteQueryAsync(userQuery, provider, defaultRowLimit, ct: cancellationToken);

        if (!queryResult.IsSuccess)
        {
            return RunResult.Error(queryResult.ErrorType);
        }

        var expected = await _dbContext.Solutions
            .Where(s => s.ChallengeId == request.ChallengeId && s.DatabaseProvider == provider)
            .Select(s => s.SolutionSql)
            .FirstOrDefaultAsync(cancellationToken); // TODO: Save expected results in cache

        if (expected is null)
            return SubmissionErrors.ChallengeNotFound;

        var expectedResult =
            await _queryExecutorDispatcher.ExecuteQueryAsync(expected, provider, defaultRowLimit, ct: cancellationToken);

        if (!expectedResult.IsSuccess)
            return RunResult.Error(expectedResult.ErrorType);

        return RunResult.FromResults(queryResult.OutputTable, expectedResult.OutputTable);
    }
}