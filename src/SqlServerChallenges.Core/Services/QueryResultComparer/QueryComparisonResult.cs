namespace SqlServerChallenges.Core.Services.QueryResultComparer;

public sealed record QueryComparisonResult(
    bool Matched,
    int ExpectedRows,
    int ActualRows,
    int? FirstMismatchRow);
