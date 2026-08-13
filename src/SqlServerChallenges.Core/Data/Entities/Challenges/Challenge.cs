using SqlServerChallenges.Core.Data.Entities.Categories;
using SqlServerChallenges.Core.Data.Entities.Challenges;
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;

namespace SqlServerChallenges.Core.Data.Entities;

public sealed class Challenge
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Slug { get; init; }
    public required int Number { get; init; }
    public required string TaskDescription { get; init; }
    public required bool RequiresOrdering { get; init; }
    public required ChallengeDifficulty Difficulty { get; init; }
    public int CategoryId { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public ICollection<ChallengeSolution> Solutions { get; init; } = [];
    public Category Category { get; init; } = null!;
}