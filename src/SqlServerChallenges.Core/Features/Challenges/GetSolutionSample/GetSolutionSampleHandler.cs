using Microsoft.EntityFrameworkCore;
using SqlServerChallenges.Core.Common.CQRS.Query;
using SqlServerChallenges.Core.Common.Results;
using SqlServerChallenges.Core.Data;
using SqlServerChallenges.Core.Services.SqlExecutor;

namespace SqlServerChallenges.Core.Features.Challenges.GetSolutionSample;

public class GetSolutionSampleHandler : IQueryHandler<GetSolutionSampleQuery, OutputTable>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly QueryExecutorDispatcher _queryExecutorDispatcher;
    
    public GetSolutionSampleHandler(QueryExecutorDispatcher queryExecutorDispatcher, ApplicationDbContext dbContext)
    {
        _queryExecutorDispatcher = queryExecutorDispatcher;
        _dbContext = dbContext;
    }

    public async Task<Result<OutputTable>> Handle(GetSolutionSampleQuery request, CancellationToken cancellationToken)
    {
        var provider = DatabaseProvider.SqlServer;
        
        var solutionSql = await _dbContext.Solutions
            .Where(s => s.ChallengeId == request.ChallengeId && s.DatabaseProvider == provider)
            .Select(s => s.SolutionSql)
            .FirstOrDefaultAsync(cancellationToken);

        if (solutionSql is null)
            return ChallengesErrors.UnknownError;

        var queryResult = await _queryExecutorDispatcher.ExecuteQueryAsync(solutionSql, provider, rowLimit: 3, ct: cancellationToken);

        if (!queryResult.IsSuccess)
            return ChallengesErrors.UnknownError;
        
        return queryResult.OutputTable;
    }
}