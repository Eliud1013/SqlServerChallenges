using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SqlServerChallenges.Core.Common.Cache;
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
    private readonly QueryExecutorDispatcher _executorDispatcher;
    private readonly ILogger<RunUserSqlHandler> _logger;
    private readonly IMemoryCache _cache;

    public RunUserSqlHandler(
        ApplicationDbContext dbContext,
        SyntaxCheckerDispatcher syntaxCheckerDispatcher,
        QueryExecutorDispatcher executorDispatcher,
        ILogger<RunUserSqlHandler> logger,
        IMemoryCache cache)
    {
        _dbContext = dbContext;
        _executorDispatcher = executorDispatcher;
        _logger = logger;
        _cache = cache;
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
        var dbProvider = request.Provider;

        var syntaxValidationResult = _syntaxCheckerDispatcher.Validate(userQuery, dbProvider);

        if (syntaxValidationResult.IsInvalid)
            return RunResult.FromSyntaxError(syntaxValidationResult.Errors);

        var queryExecutor = _executorDispatcher.GetQueryExecutor(dbProvider);
        
        var queryResult = await queryExecutor
            .ExecuteQueryAsync(userQuery, defaultRowLimit, includePlan: true, ct: cancellationToken);

        if (!queryResult.IsSuccess)
            return RunResult.FromQueryError(queryResult.ErrorType, queryResult.ErrorMessage);

        var queryTable = queryResult.OutputTable;

        if (!challenge.RequiresOrdering && queryTable.Rows.Any())
            queryTable.OrderRows();

        var cacheKey = CacheKeys.Challenges.ExpectedOutput(request.ChallengeId, dbProvider);

        if (!_cache.TryGetValue(cacheKey, out OutputTable? expectedOutput))
        {
            var expected = await _dbContext.Solutions
                .Where(s => s.ChallengeId == request.ChallengeId && s.DatabaseProvider == dbProvider)
                .Select(s => s.SolutionSql)
                .FirstOrDefaultAsync(cancellationToken);

            if (expected is null)
                return SubmissionErrors.ChallengeNotFound;

            var expectedResult =
                await queryExecutor.ExecuteQueryAsync(
                    query: expected,
                    rowLimit: defaultRowLimit,
                    includePlan: true,
                    ct: cancellationToken);

            if (!expectedResult.IsSuccess)
            {
                _logger.LogCritical(
                    $"Solution query execution failed. challengeId: {challenge.Id} provider: {dbProvider}");
                return RunResult.FromQueryError(expectedResult.ErrorType, "An error occurred");
            }

            if (!challenge.RequiresOrdering)
                expectedResult.OutputTable.OrderRows();

            expectedOutput = expectedResult.OutputTable;

            _cache.Set(cacheKey, expectedOutput, new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.High,
                Size = 0,
            });
        }

        return RunResult.FromResults(queryTable, expectedOutput!);
    }
}