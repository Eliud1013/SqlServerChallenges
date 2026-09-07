using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;
using SqlServerChallenges.Core.Services.TableReferenceExtractor;

namespace SqlServerChallenges.Core.Services.TableReferenceExtractor;

public class MsSqlTableReferencesExtractor : ITableReferencesExtractor
{
    public DatabaseProvider Provider => DatabaseProvider.SqlServer;

    public IDictionary<string, List<string>> Extract(string sql)
    {
        var parser = new TSql160Parser(initialQuotedIdentifiers: true);
        using var reader = new StringReader(sql);
        var fragment = parser.Parse(reader, out _);

        var cteNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        fragment.Accept(new CteCollectorVisitor(cteNames));

        var tableVisitor = new TableReferenceVisitor(cteNames);
        fragment.Accept(tableVisitor);

        return tableVisitor.Schemas;
    }

    private sealed class CteCollectorVisitor : TSqlFragmentVisitor
    {
        private readonly HashSet<string> _names;

        public CteCollectorVisitor(HashSet<string> names) => _names = names;

        public override void Visit(CommonTableExpression node)
        {
            _names.Add(node.ExpressionName.Value);
            base.Visit(node);
        }
    }

    private sealed class TableReferenceVisitor : TSqlFragmentVisitor
    {
        private readonly HashSet<string> _ctes;

        public Dictionary<string, List<string>> Schemas { get; } = [];

        public TableReferenceVisitor(HashSet<string> ctes) => _ctes = ctes;

        public override void Visit(NamedTableReference node)
        {
            var tableName = node.SchemaObject.BaseIdentifier.Value;

            if (!_ctes.Contains(tableName) && !tableName.StartsWith('#'))
            {
                var schemaName = node.SchemaObject.SchemaIdentifier?.Value
                                 ?? "dbo";

                if (!Schemas.TryGetValue(schemaName, out var schema))
                {
                    Schemas[schemaName] = new List<string>
                    {
                        tableName
                    };
                }
                else
                {
                    schema.Add(tableName);
                }
            }
            
            base.Visit(node);
        }
    }
}