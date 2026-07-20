using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using SqlServerChallenges.Core.Common.CQRS;
using SqlServerChallenges.Core.Common.Results;
using SqlServerChallenges.Core.Data;
using SqlServerChallenges.Core.Services;
using SqlServerChallenges.Core.Services.SqlExecutor;

namespace SqlServerChallenges.Core.Features.Challenges.SubmitSolution;

public class SubmitSolutionHandler : ICommandHandler<SubmitSolutionCommand>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly SyntaxCheckerDispatcher _syntaxCheckerDispatcher; 

    public SubmitSolutionHandler(
        ApplicationDbContext dbContext,
        MsSqlQuerySyntaxChecker querySyntaxChecker, SyntaxCheckerDispatcher syntaxCheckerDispatcher)
    {
        _dbContext = dbContext;
        _syntaxCheckerDispatcher = syntaxCheckerDispatcher;
    }

    public async Task<Result> Handle(SubmitSolutionCommand request, CancellationToken cancellationToken)
    {
        var challenge = await _dbContext.Challenges
            .FirstOrDefaultAsync(c => c.Id == request.ChallengeId, cancellationToken);

        if (challenge is null)
            return ChallengesErrors.NotFound;
        
        var userSql = request.UserQuery;
 
        
        return Result.Success();
    }
}