using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;

namespace SqlServerChallenges.Core.Services;

public interface IQuerySyntaxChecker
{
    DatabaseProvider Provider { get; }
    SqlSyntaxResult Validate(string query);
}