namespace SqlServerChallenges.Core.Data.Entities.Categories;

public sealed class Category
{
    public int Id { get; init; }
    public required string Name { get; init; }
    
    public ICollection<Challenge>? Challenges { get; init; }
}