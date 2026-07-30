namespace SqlServerChallenges.Core.Features.Challenges.GetSolutionSample;

public record SampleOutput(
    IReadOnlyList<string> Columns,
    IReadOnlyList<IReadOnlyDictionary<string, object?>> Rows);