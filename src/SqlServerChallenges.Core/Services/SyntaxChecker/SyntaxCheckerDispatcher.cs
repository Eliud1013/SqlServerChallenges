using SqlServerChallenges.Core.Services.SqlExecutor;

namespace SqlServerChallenges.Core.Services;

public class SyntaxCheckerDispatcher
{
    private readonly Dictionary<DatabaseProvider, IQuerySyntaxChecker> _checkers;

    public SyntaxCheckerDispatcher(IEnumerable<IQuerySyntaxChecker> syntaxCheckers)
    {
        _checkers = syntaxCheckers.ToDictionary(x => x.Provider);
    }

    public SqlSyntaxResult Validate(string query, DatabaseProvider provider)
    {
        if (!_checkers.TryGetValue(provider, out var checker))
            throw new InvalidOperationException();

        return checker.Validate(query);
    }
}