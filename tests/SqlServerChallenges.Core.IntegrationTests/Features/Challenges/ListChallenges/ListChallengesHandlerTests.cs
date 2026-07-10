using FluentAssertions;
using Org.BouncyCastle.Asn1.Cms;
using SqlServerChallenges.Core.Data;
using SqlServerChallenges.Core.Data.Entities;
using SqlServerChallenges.Core.Data.Entities.Categories;
using SqlServerChallenges.Core.Data.Entities.Challenges;
using SqlServerChallenges.Core.Features.Challenges.ListChallenges;
using SqlServerChallenges.Core.IntegrationTests.Common;
using Xunit;

namespace SqlServerChallenges.Core.IntegrationTests.Features.Challenges.ListChallenges;

public class ListChallengesHandlerTests : IClassFixture<SqlServerFixture>, IAsyncLifetime
{
    private readonly SqlServerFixture _fixture;
    private readonly ApplicationDbContext _dbContext;

    public ListChallengesHandlerTests(SqlServerFixture fixture)
    {
        _fixture = fixture;
        _dbContext = _fixture.CreateDbContext();
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await _fixture.ClearDatabaseAsync(_dbContext);

    [Fact]
    public async Task ShouldReturnAllChallenges_WhenNoFiltersAreProvided()
    {
        _dbContext.Challenges.Add(new Challenge
        {
            Id = Guid.NewGuid(),
            Title = "Basic SELECT",
            TaskDescription = "Write a SELECT query",
            SolutionQuery = "SELECT 1",
            Difficulty = ChallengeDifficulty.Easy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();

        var handler = new ListChallengesHandler(_dbContext);

        var result = await handler.Handle(
            new ListChallengesQuery(null, null, null),
            CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Value.Should().ContainSingle(c => c.Title == "Basic SELECT");
    }

    [Fact]
    public async Task ShouldFilterByCategory_WhenCategoryIsProvided()
    {
        _dbContext.Challenges.Add(new Challenge
        {
            Id = Guid.NewGuid(),
            Title = "Basic SELECT",
            TaskDescription = "Write a SELECT query",
            SolutionQuery = "SELECT 1",
            Difficulty = ChallengeDifficulty.Easy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Categories = new List<Category>
            {
                new Category
                {
                    Name = "Basic SELECT"
                }
            },
        });

        await _dbContext.SaveChangesAsync();

        var handler = new ListChallengesHandler(_dbContext);

        var result = await handler.Handle(new ListChallengesQuery(null, "Basic SELECT", null), CancellationToken.None);
        
        result.Should().NotBeNull();
        result.Value.Should().ContainSingle(c => c.Title == "Basic SELECT");
    }
}