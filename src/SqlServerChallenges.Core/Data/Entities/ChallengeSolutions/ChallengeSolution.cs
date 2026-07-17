using SqlServerChallenges.Core.Services.SqlExecutor;

namespace SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;

public class ChallengeSolution
{
    public Guid ChallengeId { get; init; }
    public Challenge? Challenge { get; init; }
    public required DatabaseProvider DatabaseProvider { get; init; }
    
    public required string SolutionSql { get; init; }
    
}