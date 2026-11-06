using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;
using SqlServerChallenges.Core.Data.Entities.Submissions;

namespace SqlServerChallenges.Core.Data.Entities.QueryPlans;

public class SubmissionPlan
{
    public Guid Id { get; init; }
    public Guid SubmissionId { get; init; }
    public required DatabaseProvider Provider { get; init; }
    public required string? PhysicalOperation { get; init; }
    public required string? LogicalOperation { get; init; }
    public required double? EstimateIO { get; init; }
    public required double? EstimateCPU { get; init; }
}