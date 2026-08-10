using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using SqlServerChallenges.Core.Data.Entities;
using SqlServerChallenges.Core.Data.Entities.Categories;
using SqlServerChallenges.Core.Data.Entities.Challenges;
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;
using SqlServerChallenges.Core.Services.QueryExecutor;
using SqlServerChallenges.Core.Services.SampleOutput;
using SqlServerChallenges.Core.Tests.Common;
using Xunit;

namespace SqlServerChallenges.Core.Tests.Services.SampleOutput;

public class SampleOutputProviderTests : BaseIntegrationTest
{
    public SampleOutputProviderTests(SqlServerFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task ShouldRespectRowLimit_WhenProvided()
    {
        var challengeId = Guid.NewGuid();
        var cache = new MemoryCache(new MemoryCacheOptions());
        var queryExecutorDispatcher = new QueryExecutorDispatcher(new List<IQueryExecutor>
        {
            new MsSqlQueryExecutor(new SqlConnection(_fixture._connectionString))
        });
        
        _dbContext.Challenges.Add(new Challenge
        {
            Id = challengeId,
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
                    ChallengeId = challengeId,
                    DatabaseProvider = DatabaseProvider.SqlServer,
                    SolutionSql = "SELECT value from generate_series(1,100)"
                }
            }
        });
        
        await _dbContext.SaveChangesAsync();
        
        var outputSample = new SampleOutputProvider(cache, _dbContext, queryExecutorDispatcher);
        var result = await outputSample.GetForChallengeAsync(challengeId, 3);
        
        result.Succeeded.Should().BeTrue();
        result.Value.Rows.Count.Should().Be(3);
        result.Value.Columns.Count.Should().Be(1);
    }
}