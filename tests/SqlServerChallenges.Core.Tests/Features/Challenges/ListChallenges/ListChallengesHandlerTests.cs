using FluentAssertions;
using SqlServerChallenges.Core.Data;
using SqlServerChallenges.Core.Data.Entities;
using SqlServerChallenges.Core.Data.Entities.Categories;
using SqlServerChallenges.Core.Data.Entities.Challenges;
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;
using SqlServerChallenges.Core.Features.Challenges.ListChallenges;
using SqlServerChallenges.Core.Tests.Common;
using Xunit;

namespace SqlServerChallenges.Core.Tests.Features.Challenges.ListChallenges;

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
            Slug = "basic-select",
            TaskDescription = "Write a SELECT query",
            Difficulty = ChallengeDifficulty.Easy,
            RequiresOrdering = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Category = new Category { Name = "Basic Queries" }
        });

        await _dbContext.SaveChangesAsync();

        var handler = new ListChallengesHandler(_dbContext);

        var result = await handler.Handle(
            new ListChallengesQuery(null, null, null),
            CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Value.Should().BeOfType<List<ChallengeEntry>>();
        result.Value.Should().ContainSingle(c => c.Title == "Basic SELECT");
    }

    [Fact]
    public async Task ShouldFilterByCategory_WhenCategoryIsProvided()
    {
        _dbContext.Challenges.Add(new Challenge
        {
            Id = Guid.NewGuid(),
            Title = "Basic SELECT",
            Slug = "basic-select",
            TaskDescription = "Write a SELECT query",
            Difficulty = ChallengeDifficulty.Easy,
            RequiresOrdering = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Category = new Category
            {
                Name = "Basic SELECT"
            }
        });

        await _dbContext.SaveChangesAsync();

        var handler = new ListChallengesHandler(_dbContext);

        var result = await handler.Handle(new ListChallengesQuery(null, "Basic SELECT", null), CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Value.Should().ContainSingle(c => c.Title == "Basic SELECT");
        result.Value.Select(v => v.Category).Should().NotBeEmpty();
    }

    [Fact]
    public async Task ShouldFilterByTitle_WhenTitleIsProvided()
    {
        _dbContext.Challenges.Add(new Challenge
        {
            Id = Guid.NewGuid(),
            Title = "Basic SELECT",
            Slug = "basic-select",
            TaskDescription = "Write a SELECT query",
            Difficulty = ChallengeDifficulty.Easy,
            RequiresOrdering = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Category = new Category { Name = "Basic Queries" },
            Solutions = new List<ChallengeSolution>
            {
                new ChallengeSolution
                {
                    DatabaseProvider = DatabaseProvider.SqlServer,
                    SolutionSql = "SELECT 1"
                },
            }
        });

        await _dbContext.SaveChangesAsync();

        var handler = new ListChallengesHandler(_dbContext);

        var result = await handler.Handle(new ListChallengesQuery("Basic", null, null), CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Value.Should().ContainSingle(c => c.Title == "Basic SELECT");
    }

    [Fact]
    public async Task ShouldFilterByDifficulty_WhenDifficultyIsProvided()
    {
        _dbContext.Challenges.Add(new Challenge
        {
            Id = Guid.NewGuid(),
            Title = "Basic SELECT",
            Slug = "basic-select",
            TaskDescription = "Write a SELECT query",
            Difficulty = ChallengeDifficulty.Medium,
            RequiresOrdering = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Category = new Category { Name = "Basic Queries" },
            Solutions = new List<ChallengeSolution>()
            {
                new ChallengeSolution
                {
                    DatabaseProvider = DatabaseProvider.SqlServer,
                    SolutionSql = "SELECT 1"
                },
            }
        });

        await _dbContext.SaveChangesAsync();

        var handler = new ListChallengesHandler(_dbContext);

        var result = await handler.Handle(new ListChallengesQuery(null, null, ChallengeDifficulty.Medium),
            CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Value.Should()
            .ContainSingle(c => c.Title == "Basic SELECT" && c.Difficulty == ChallengeDifficulty.Medium);
    }
}