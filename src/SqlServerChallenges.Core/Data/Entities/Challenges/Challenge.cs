using SqlServerChallenges.Core.Data.Entities.Categories;
using SqlServerChallenges.Core.Data.Entities.Challenges;

namespace SqlServerChallenges.Core.Data.Entities;

public sealed class Challenge
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public required string TaskDescription { get; init; }
    public required string SolutionQuery { get; init; }
    public required ChallengeDifficulty Difficulty { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }

    public ICollection<Category> Categories { get; init; } = [];
}