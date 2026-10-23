using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SqlServerChallenges.Core.Common.Cache;
using SqlServerChallenges.Core.Common.CQRS;
using SqlServerChallenges.Core.Common.Results;
using SqlServerChallenges.Core.Data;
using SqlServerChallenges.Core.Services;
using SqlServerChallenges.Core.Services.ConnectionProvider;
using SqlServerChallenges.Core.Services.QueryExecutor;

namespace SqlServerChallenges.Core.Features.Submissions.SubmitSolution;

public class SubmitSolutionHandler : ICommandHandler<SubmitSolutionCommand, SubmissionResult>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly SyntaxCheckerDispatcher _syntaxCheckerDispatcher;
    private readonly IDbConnectionProvider _connectionProvider;
    private readonly IMemoryCache _cache;

    public SubmitSolutionHandler(
        ApplicationDbContext dbContext,
        SyntaxCheckerDispatcher syntaxCheckerDispatcher,
        QueryExecutorDispatcher executorDispatcher,
        IDbConnectionProvider connectionProvider,
        IMemoryCache cache)
    {
        _dbContext = dbContext;
        _syntaxCheckerDispatcher = syntaxCheckerDispatcher;
        _cache = cache;
        _connectionProvider = connectionProvider;
    }

    public async Task<Result<SubmissionResult>> Handle(SubmitSolutionCommand request,
        CancellationToken cancellationToken)
    {
        var userQuery = request.UserQuery;
        var dbProvider = request.Provider;

        var challenge = await _dbContext.Challenges
            .Include(challenge => challenge.Solutions)
            .FirstOrDefaultAsync(c => c.Id == request.ChallengeId, cancellationToken);

        if (challenge is null)
            return SubmissionErrors.ChallengeNotFound;

        var syntaxValidationResult = _syntaxCheckerDispatcher.Validate(userQuery, dbProvider);

        if (syntaxValidationResult.IsInvalid)
            return SubmissionResult.FromSyntaxError(syntaxValidationResult.Errors);

        var dbConnection = _connectionProvider.GetConnection(dbProvider);
        var solutionKey = CacheKeys.Challenges.Solution(request.ChallengeId, dbProvider);
        
        if (!_cache.TryGetValue(solutionKey, out Dictionary<string, int>? solutionDict))
        {
            var challengeSolution = challenge.Solutions
                .FirstOrDefault(s => s.DatabaseProvider == dbProvider);

            if (challengeSolution is null)
                return SubmissionErrors.SolutionNotFound;

            string solutionSql = challengeSolution.SolutionSql;

            var rowsCount = new Dictionary<string, int>();
            await using var solutionReader = await GetCommandReaderAsync(solutionSql, dbConnection, cancellationToken);
            
            while (await solutionReader.ReadAsync(cancellationToken))
                rowsCount[GetKey(solutionReader)] = rowsCount.GetValueOrDefault(GetKey(solutionReader)) + 1;

            solutionDict = rowsCount;
            
            _cache.Set(solutionKey, rowsCount, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(1)
            });
        }

        var solutionRows = new Dictionary<string, int>(solutionDict!);
        await using var userReader = await GetCommandReaderAsync(userQuery, dbConnection, cancellationToken);
        
        while (await userReader.ReadAsync(cancellationToken))
        {
            var k = GetKey(userReader);
            if (solutionRows.GetValueOrDefault(k) == 0) 
                return SubmissionResult.FromComparisionResult(passed: false);
            
            solutionRows[k]--;
        }
        
        if(solutionRows.Values.Any(v => v != 0))
            return SubmissionResult.FromComparisionResult(passed: false);
        
        return SubmissionResult.FromComparisionResult(passed: true);
    }
    
    private static string GetKey(DbDataReader r)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < r.FieldCount; i++)
        {
            if (r.IsDBNull(i)) { sb.Append("-1:"); continue; }
            var s = Convert.ToString(r.GetValue(i), CultureInfo.InvariantCulture) ?? "";
            sb.Append(s.Length).Append(':').Append(s);
        }
        
        byte[] input = Encoding.UTF8.GetBytes(sb.ToString());
        byte[] hash = SHA256.HashData(input);
        string key = Convert.ToHexString(hash);

        return key;
    }

    private async Task<DbDataReader> GetCommandReaderAsync(
        string query,
        DbConnection connection,
        CancellationToken ct = default)
    {
        if (connection.State == ConnectionState.Closed)
            await connection.OpenAsync(ct);

        var command = connection.CreateCommand();
        command.CommandText = query;
        command.CommandTimeout = 6;

        return await command.ExecuteReaderAsync(ct);
    }
}