using System.Data.Common;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SqlServerChallenges.Core.Authentication;
using SqlServerChallenges.Core.Common.Cache;
using SqlServerChallenges.Core.Common.CQRS;
using SqlServerChallenges.Core.Common.Results;
using SqlServerChallenges.Core.Data;
using SqlServerChallenges.Core.Data.Entities.Submissions;
using SqlServerChallenges.Core.Services;
using SqlServerChallenges.Core.Services.QueryExecutor;

namespace SqlServerChallenges.Core.Features.Submissions.SubmitSolution;

public class SubmitSolutionHandler : ICommandHandler<SubmitSolutionCommand, SubmissionResult>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly QueryExecutorDispatcher _executorDispatcher;
    private readonly SyntaxCheckerDispatcher _syntaxCheckerDispatcher;
    private readonly IMemoryCache _cache;
    private readonly IUserContext _userContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<SubmitSolutionHandler> _logger;

    public SubmitSolutionHandler(
        ApplicationDbContext dbContext,
        QueryExecutorDispatcher executorDispatcher,
        SyntaxCheckerDispatcher syntaxCheckerDispatcher,
        IMemoryCache cache,
        IUserContext userContext,
        IDateTimeProvider dateTimeProvider,
        ILogger<SubmitSolutionHandler> logger)
    {
        _dbContext = dbContext;
        _executorDispatcher = executorDispatcher;
        _syntaxCheckerDispatcher = syntaxCheckerDispatcher;
        _cache = cache;
        _userContext = userContext;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<Result<SubmissionResult>> Handle(
        SubmitSolutionCommand request,
        CancellationToken ct)
    {
        var userQuery = request.UserQuery; // TODO: Validate input length
        var dbProvider = request.Provider;
        var challengeId = request.ChallengeId;

        var challenge = await _dbContext.Challenges
            .Include(challenge => challenge.Solutions)
            .FirstOrDefaultAsync(c => c.Id == challengeId, ct);

        if (challenge is null)
            return SubmissionErrors.ChallengeNotFound;

        var syntaxValidationResult = _syntaxCheckerDispatcher.Validate(userQuery, dbProvider);

        if (syntaxValidationResult.IsInvalid)
        {
            await SaveSubmissionResultAsync(
                challengeId: challengeId,
                passed: false,
                sql: userQuery,
                outcome: SubmissionOutcome.SyntaxError,
                ct);
            
            return SubmissionResult.FromSyntaxError(syntaxValidationResult.Errors);
        }

        var solutionKey = CacheKeys.Challenges.Solution(request.ChallengeId, dbProvider);
        var queryExecutor = _executorDispatcher.GetQueryExecutor(dbProvider);

        if (!_cache.TryGetValue(solutionKey, out Dictionary<string, int>? solutionDict))
        {
            var challengeSolution = challenge.Solutions
                .FirstOrDefault(s => s.DatabaseProvider == dbProvider);

            if (challengeSolution is null)
            {
                _logger.LogCritical($"Solution for challengeId {challengeId} [{dbProvider}] not found");
                return SubmissionErrors.SolutionNotFound;
            }

            string solutionSql = challengeSolution.SolutionSql;

            var rowsCount = new Dictionary<string, int>();
            await using var solutionReaderResult = await queryExecutor.ExecuteReaderAsync(solutionSql, ct);

            if (!solutionReaderResult.IsSuccess)
            {
                _logger.LogCritical($"Solution reader execution for challengeId {challengeId} [{dbProvider}] failed");
                return SubmissionErrors.UnknownError;
            }

            var solutionReader = solutionReaderResult.DataReader;

            while (await solutionReader.ReadAsync(ct))
            {
                var key = GetKey(solutionReader);
                rowsCount[key] = rowsCount.GetValueOrDefault(key) + 1;
            }

            solutionDict = rowsCount;

            _cache.Set(solutionKey, rowsCount, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(1)
            });
        }

        var solutionRows = new Dictionary<string, int>(solutionDict!);
        await using var userReaderResult = await queryExecutor.ExecuteReaderAsync(userQuery, ct);

        if (!userReaderResult.IsSuccess)
        {
            if (userReaderResult.ErrorType == QueryErrorType.Unknown)
            {
                _logger.LogError(
                    "Failed to execute user-submitted query for challenge {ChallengeId} " +
                    "for provider {DatabaseProvider}. ErrorType: {ErrorType}. ErrorMessage: {ErrorMessage}",
                    challengeId,
                    dbProvider,
                    userReaderResult.ErrorType,
                    userReaderResult.ErrorMessage);

                return SubmissionErrors.UnknownError;
            }

            var failureOutcome = MapQueryErrorToOutcome(userReaderResult.ErrorType);

            await SaveSubmissionResultAsync(
                challengeId: challengeId,
                passed: false,
                sql: userQuery,
                outcome: failureOutcome,
                ct: ct);

            return SubmissionResult.FromQueryError(userReaderResult.QueryError);
        }

        var userReader = userReaderResult.DataReader;

        while (await userReader.ReadAsync(ct))
        {
            var k = GetKey(userReader);
            if (solutionRows.GetValueOrDefault(k) == 0)
            {
                solutionRows[k] = -1;
                break;
            }

            solutionRows[k]--;
        }

        var passed = solutionRows.Values.All(v => v == 0);
        var submissionOutcome = passed
            ? SubmissionOutcome.Accepted
            : SubmissionOutcome.WrongAnswer;

        await SaveSubmissionResultAsync(
            challengeId: challengeId,
            passed: passed,
            sql: userQuery,
            outcome: submissionOutcome,
            ct: ct);

        return SubmissionResult.FromComparisionResult(passed);
    }

    private async Task SaveSubmissionResultAsync(
        Guid challengeId,
        bool passed,
        string sql,
        SubmissionOutcome outcome,
        CancellationToken ct)
    {
        _dbContext.Submissions.Add(new Submission
        {
            ChallengeId = challengeId,
            UserId = _userContext.UserId!,
            SubmittedSql = sql,
            Passed = passed,
            Outcome = outcome,
            SubmittedAtUtc = _dateTimeProvider.UtcNow,
        });
        await _dbContext.SaveChangesAsync(ct);
    }


    private static SubmissionOutcome MapQueryErrorToOutcome(QueryErrorType errorType)
    {
        return errorType switch
        {
            QueryErrorType.QueryTimeout => SubmissionOutcome.QueryTimeout,

            QueryErrorType.PermissionDenied => SubmissionOutcome.QueryExecutionError,

            QueryErrorType.InvalidQuery => SubmissionOutcome.QueryExecutionError,

            QueryErrorType.InvalidIdentifier => SubmissionOutcome.QueryExecutionError,

            _ => throw new InvalidOperationException(
                $"No submission outcome mapping exists for query error type '{errorType}'.")
        };
    }

    private string GetKey(DbDataReader r)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < r.FieldCount; i++)
        {
            if (r.IsDBNull(i))
            {
                sb.Append("-1:");
                continue;
            }

            var s = Convert.ToString(r.GetValue(i), CultureInfo.InvariantCulture) ?? "";
            sb.Append(s.Length).Append(':').Append(s);
        }

        byte[] input = Encoding.UTF8.GetBytes(sb.ToString());
        byte[] hash = SHA256.HashData(input);
        string key = Convert.ToHexString(hash);

        return key;
    }
}