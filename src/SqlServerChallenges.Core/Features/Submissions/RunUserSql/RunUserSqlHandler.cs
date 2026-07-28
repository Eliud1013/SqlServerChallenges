using System.Data;
using System.Security.AccessControl;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using SqlServerChallenges.Core.Common.CQRS;
using SqlServerChallenges.Core.Common.Results;
using SqlServerChallenges.Core.Data;
using SqlServerChallenges.Core.Features.Challenges;
using SqlServerChallenges.Core.Services;
using SqlServerChallenges.Core.Services.SqlExecutor;

namespace SqlServerChallenges.Core.Features.Submissions.RunUserQuery;

public class RunUserSqlHandler : ICommandHandler<RunUserSqlCommand, QueryExecutorResult>
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

    public async Task<Result<QueryExecutorResult>> Handle(RunUserSqlCommand request,
        CancellationToken cancellationToken)
    {
        var challenge = await _dbContext.Challenges
            .FirstOrDefaultAsync(c => c.Id == request.ChallengeId, cancellationToken);

        if (challenge is null)
            return SubmissionErrors.ChallengeNotFound;

        var userQuery = request.UserQuery;
        var provider = request.provider;

        var syntaxValidationResult = _syntaxCheckerDispatcher.Validate(userQuery, provider);

        if (syntaxValidationResult.IsInvalid)
            return SubmissionErrors.SyntaxError(syntaxValidationResult.Errors);

        var queryResult = await _queryExecutorDispatcher.ExecuteQueryAsync(userQuery, provider, ct: cancellationToken);

        return queryResult;
    }
}