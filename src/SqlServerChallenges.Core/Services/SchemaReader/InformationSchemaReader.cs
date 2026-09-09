using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using Microsoft.Data.SqlClient;
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;
using SqlServerChallenges.Core.Services.ConnectionProvider;

namespace SqlServerChallenges.Core.Services.SchemaReader;

public record ColumnInfo(string Name, string Type, bool IsNullable);

public class InformationSchemaReader
{
    private readonly IDbConnectionProvider _connectionProvider;
    
    public InformationSchemaReader(IDbConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }
    
    public async Task<IReadOnlyDictionary<string, IReadOnlyList<ColumnInfo>>> ReadColumnsAsync(
        IReadOnlyDictionary<string, List<string>> tables,
        DatabaseProvider provider,
        CancellationToken cancellationToken)
    {
        if (tables.Count == 0)
            return new Dictionary<string, IReadOnlyList<ColumnInfo>>();

        DbProviderFactory factory = provider switch
        {
            DatabaseProvider.SqlServer => SqlClientFactory.Instance,
            _ => throw new InvalidOperationException("Invalid provider")
        };

        var predicates = new StringBuilder();
        var parameters = new List<DbParameter>();
        int paramIdx = 0;

        foreach (var (schema, names) in tables)
        {
            foreach (var name in names)
            {
                if (predicates.Length > 0)
                    predicates.Append(" OR ");

                predicates.Append(
                    $"(TABLE_SCHEMA = @schema_{paramIdx} AND TABLE_NAME = @table_{paramIdx})");

                var schemaParam = factory.CreateParameter()!;
                schemaParam.ParameterName = $"@schema_{paramIdx}";
                schemaParam.Value = schema;
                parameters.Add(schemaParam);

                var tableParam = factory.CreateParameter()!;
                tableParam.ParameterName = $"@table_{paramIdx}";
                tableParam.Value = name;
                parameters.Add(tableParam);

                paramIdx++;
            }
        }

        var sql = $"""
                   SELECT TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME, DATA_TYPE, IS_NULLABLE, ORDINAL_POSITION
                   FROM INFORMATION_SCHEMA.COLUMNS
                   WHERE {predicates}
                   ORDER BY TABLE_SCHEMA, TABLE_NAME, ORDINAL_POSITION;
                   """;

        var connection = _connectionProvider.GetConnection(provider);
        await using var command = factory.CreateCommand()!;
        command.CommandText = sql;
        command.Connection = connection;
        command.Parameters.AddRange(parameters.ToArray());

        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync(cancellationToken);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var tableDefinitions = new Dictionary<string, List<ColumnInfo>>();

        while (await reader.ReadAsync(cancellationToken))
        {
            var tableIdentifier = $"{reader.GetString(0)}.{reader.GetString(1)}";

        
            if (!tableDefinitions.TryGetValue(tableIdentifier, out var columns))
            {
                columns = [];
                tableDefinitions[tableIdentifier] = columns;
            }

            columns.Add(new ColumnInfo(
                Name: reader.GetString(2),
                Type: reader.GetString(3),
                IsNullable: reader.GetString(4) == "YES")
            );
            
        }

        return tableDefinitions.ToDictionary(
            x => x.Key,
            x => (IReadOnlyList<ColumnInfo>)x.Value
        );
    }
}