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
        var challenge = new ChallengeBuilder()
            .Build();

        _dbContext.Challenges.Add(challenge);

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
        var challenge = new ChallengeBuilder()
            .WithTitle("Basic select")
            .WithCategory(new Category
            {
                Name = "SELECT"
            })
            .Build();

        _dbContext.Challenges.AddRange(challenge, new ChallengeBuilder().Build());
        await _dbContext.SaveChangesAsync();

        var handler = new ListChallengesHandler(_dbContext);

        var result = await handler.Handle(new ListChallengesQuery(null, CategoryName: "SELECT", null),
            CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Value.Should().ContainSingle(c => c.Title == "Basic select");
        result.Value.Select(v => v.Category).Should().NotBeEmpty();
        result.Value.Select(v => v.Category).Should().BeEqualTo("SELECT");
    }

    [Fact]
    public async Task ShouldFilterByTitle_WhenTitleIsProvided()
    {
        var challenge = new ChallengeBuilder()
            .WithTitle("Basic select")
            .Build();

        _dbContext.Challenges.Add(challenge);
        await _dbContext.SaveChangesAsync();

        var handler = new ListChallengesHandler(_dbContext);

        var result = await handler.Handle(new ListChallengesQuery("Basic select", null, null), CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Value.Should().ContainSingle(c => c.Title == "Basic select");
    }

    [Fact]
    public async Task ShouldFilterByDifficulty_WhenDifficultyIsProvided()
    {
        var challenge = new ChallengeBuilder()
            .WithDifficulty(ChallengeDifficulty.Medium)
            .Build();

        _dbContext.Challenges.Add(challenge);
        await _dbContext.SaveChangesAsync();

        var handler = new ListChallengesHandler(_dbContext);

        var result = await handler.Handle(new ListChallengesQuery(null, null, ChallengeDifficulty.Medium),
            CancellationToken.None);

        result.Succeeded.Should().BeTrue();
        result.Value.Should()
            .ContainSingle(c => c.Difficulty == ChallengeDifficulty.Medium);
    }
}