using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SqlServerChallenges.Core.Common.Cache;
using SqlServerChallenges.Core.Common.Results;
using SqlServerChallenges.Core.Data;
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;
using SqlServerChallenges.Core.Features.Challenges;
using SqlServerChallenges.Core.Services.QueryExecutor;

namespace SqlServerChallenges.Core.Services.SampleOutput;

public class SampleOutputProvider : ISampleOutputProvider
{
    private readonly IMemoryCache _cache;
    private readonly ApplicationDbContext _dbContext;
    private readonly QueryExecutorDispatcher _queryExecutorDispatcher;

    public SampleOutputProvider(
        IMemoryCache cache,
        ApplicationDbContext dbContext,
        QueryExecutorDispatcher queryExecutorDispatcher)
    {
        _cache = cache;
        _dbContext = dbContext;
        _queryExecutorDispatcher = queryExecutorDispatcher;
    }

    public async Task<Result<OutputTable>> GetForChallengeAsync(Guid challengeId, int rowLimit, CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeys.Challenges.SolutionSample(challengeId, rowLimit);

        if (_cache.TryGetValue(cacheKey, out object? cached) && cached is OutputTable hit)
            return Result.Success(hit);

        var provider = DatabaseProvider.SqlServer;
        
        var challengeExists = await _dbContext.Challenges
            .AnyAsync(c => c.Id == challengeId, cancellationToken);

        if (!challengeExists)
            return Error.NullValue;

        var solutionSql = await _dbContext.Solutions
            .Where(s => s.ChallengeId == challengeId && s.DatabaseProvider == provider)
            .Select(s => s.SolutionSql)
            .FirstOrDefaultAsync(cancellationToken);

        if (solutionSql is null)
            throw new InvalidOperationException($"Solution SQL not found for challengeId: {challengeId}");

        var queryResult = await _queryExecutorDispatcher.ExecuteQueryAsync(solutionSql, provider, rowLimit, cancellationToken);

        if (!queryResult.IsSuccess)
            throw new InvalidOperationException($"Failed to execute solution query for challengeId: {challengeId} {queryResult.ErrorType}");
        
        _cache.Set(cacheKey, queryResult.OutputTable, new MemoryCacheEntryOptions
        {
            Priority =  CacheItemPriority.High
        });

        return queryResult.OutputTable;
    }
}
