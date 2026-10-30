namespace SqlServerChallenges.Core.Services.QueryExecutor;

public sealed record QueryPlanInfo(
    string? PhysicalOp,
    string? LogicalOp,
    long? ActualRows,
    double? EstimateIO,
    double? EstimateCPU,
    long? ActualCPUms,
    long? ActualElapsedMs);
