using SqlServerChallenges.Core.Services.SqlExecutor;

namespace SqlServerChallenges.Core.Services;

public interface IQuerySyntaxChecker
{
    DatabaseProvider Provider { get; }
    SqlSyntaxResult Validate(string query);
}