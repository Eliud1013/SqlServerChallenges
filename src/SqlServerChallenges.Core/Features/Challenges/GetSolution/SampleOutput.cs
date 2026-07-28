namespace SqlServerChallenges.Core.Features.Challenges.GetSolution;

public record SampleOutput(
    IReadOnlyList<string> Columns,
    IReadOnlyList<IReadOnlyDictionary<string, object?>> Rows);