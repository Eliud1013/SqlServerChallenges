using SqlServerChallenges.Core.Common.Results;
using SqlServerChallenges.Core.Services.QueryExecutor;

namespace SqlServerChallenges.Core.Services.SampleOutput;

public interface ISampleOutputProvider
{
    Task<Result<OutputTable>> GetForChallengeAsync(Guid challengeId, int rowLimit, CancellationToken cancellationToken = default);
}