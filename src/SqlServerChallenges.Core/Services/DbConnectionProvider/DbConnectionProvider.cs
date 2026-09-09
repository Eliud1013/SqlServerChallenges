using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;

namespace SqlServerChallenges.Core.Services.ConnectionProvider;

public class DbConnectionProvider : IDbConnectionProvider
{
    private SqlConnection _sqlConnection;

    public DbConnectionProvider(SqlConnection sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }

    public DbConnection GetConnection(DatabaseProvider provider)
    {
        return provider switch
        {
            DatabaseProvider.SqlServer => _sqlConnection,
            _ => throw new InvalidOperationException($"Database provider '{provider}' is not supported.")
        };
    }
}