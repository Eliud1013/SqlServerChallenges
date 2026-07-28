using SqlServerChallenges.Core.Common.CQRS.Query;
using SqlServerChallenges.Core.Services.SqlExecutor;

namespace SqlServerChallenges.Core.Features.Challenges.GetSolution;

public sealed record GetSolutionSampleQuery(Guid ChallengeId) : IQuery<SampleOutput>;