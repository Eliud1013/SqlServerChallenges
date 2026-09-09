using System.Data;
using System.Data.Common;
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;

namespace SqlServerChallenges.Core.Services.ConnectionProvider;

public interface IDbConnectionProvider
{
    DbConnection GetConnection(DatabaseProvider provider);
}