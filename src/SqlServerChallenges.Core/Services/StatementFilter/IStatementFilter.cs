namespace SqlServerChallenges.Core.Services;

public interface IStatementFilter
{
    Task<bool> ContainsSelectOnly(string sql);
}