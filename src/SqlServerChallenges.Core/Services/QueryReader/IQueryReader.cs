using System.Data.Common;
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;

namespace SqlServerChallenges.Core.Services.QueryReader;

public interface IQueryReader
{
    DatabaseProvider Provider { get; }
    Task<DbDataReader> ExecuteReaderAsync(string query, CancellationToken ct = default);
}
