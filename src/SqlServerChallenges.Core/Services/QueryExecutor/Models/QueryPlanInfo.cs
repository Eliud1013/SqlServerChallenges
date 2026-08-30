namespace SqlServerChallenges.Core.Services.QueryExecutor;

public sealed record QueryPlanInfo(
    string? PhysicalOp,
    string? LogicalOp,
    long? EstimateRows,
    double? EstimateIO,
    double? EstimateCPU);