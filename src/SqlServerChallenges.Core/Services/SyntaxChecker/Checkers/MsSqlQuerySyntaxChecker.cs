using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;

namespace SqlServerChallenges.Core.Services;

using Microsoft.SqlServer.TransactSql.ScriptDom;

public class MsSqlQuerySyntaxChecker : IQuerySyntaxChecker
{
    public DatabaseProvider Provider => DatabaseProvider.SqlServer;

    public SqlSyntaxResult Validate(string sql)
    {
        var parser = new TSql160Parser(initialQuotedIdentifiers: true);
        using var reader = new StringReader(sql);

        parser.Parse(reader, out IList<ParseError> parserErrors);

        var errors = parserErrors
            .Select(e => new SqlSyntaxError(e.Line, e.Column, e.Message))
            .ToList();

        return new SqlSyntaxResult(errors);
    }
}