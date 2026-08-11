using System.Data;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;
using SqlServerChallenges.Core.Services.QueryExecutor;
using SqlServerChallenges.Core.Services.SampleOutput;
using SqlServerChallenges.Core.Tests.Common;
using Xunit;

namespace SqlServerChallenges.Core.Tests.Services.SampleOutput;

public class SampleOutputProviderTests : BaseIntegrationTest
{
    private IMemoryCache _cache;
    private QueryExecutorDispatcher _queryExecutorDispatcher;

    public SampleOutputProviderTests(SqlServerFixture fixture) : base(fixture)
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
        _queryExecutorDispatcher = new QueryExecutorDispatcher(new List<IQueryExecutor>
        {
            new MsSqlQueryExecutor(new SqlConnection(_fixture._connectionString))
        });
    }

    [Fact]
    public async Task ShouldRespectRowLimit_WhenProvided()
    {
        var challenge = new ChallengeBuilder()
            .WithSolution("SELECT value from generate_series(1,100)", DatabaseProvider.SqlServer)
            .Build();

        _dbContext.Challenges.Add(challenge);

        await _dbContext.SaveChangesAsync();

        var outputSample = new SampleOutputProvider(_cache, _dbContext, _queryExecutorDispatcher);
        var result = await outputSample.GetForChallengeAsync(challenge.Id, 3);

        result.Succeeded.Should().BeTrue();
        result.Value.Rows.Count.Should().Be(3);
        result.Value.Columns.Count.Should().Be(1);
    }

    [Fact]
    public async Task ShouldReturnCachedValue_WhenAlreadyCached()
    {
        var challenge = new ChallengeBuilder()
            .WithSolution("SELECT value from generate_series(1,100)", DatabaseProvider.SqlServer)
            .Build();

        _dbContext.Challenges.Add(challenge);
        await _dbContext.SaveChangesAsync();

        var sqlExecutorMock = new Mock<IQueryExecutor>();

        sqlExecutorMock.Setup(m => m.Provider)
            .Returns(DatabaseProvider.SqlServer);

        sqlExecutorMock.Setup(m => m.ExecuteQueryAsync(
            It.IsAny<string>(),
            It.IsAny<int?>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(new DataTable());

        _queryExecutorDispatcher = new QueryExecutorDispatcher(new List<IQueryExecutor>
        {
            sqlExecutorMock.Object
        });

        var outputSample = new SampleOutputProvider(_cache, _dbContext, _queryExecutorDispatcher);
        
        var firstResult = await outputSample.GetForChallengeAsync(challenge.Id, 3);
        
        foreach (var _ in Enumerable.Range(0, 4))
            await outputSample.GetForChallengeAsync(challenge.Id, 3);
        
        var lastResult = await outputSample.GetForChallengeAsync(challenge.Id, 3);

        sqlExecutorMock.Verify(
            x => x.ExecuteQueryAsync(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<CancellationToken>()), Times.Once);
        
        firstResult.Succeeded.Should().BeTrue();
        lastResult.Succeeded.Should().BeTrue();
        lastResult.Value.Should().BeSameAs(firstResult.Value);
    }

    [Fact]
    public async Task ShouldThrowInvalidOperationException_WhenSolutionSqlIsNotFound()
    {
        var challenge = new ChallengeBuilder()
            .Build();
        
        _dbContext.Challenges.Add(challenge);
        await _dbContext.SaveChangesAsync();
        
        var outputSample = new SampleOutputProvider(_cache, _dbContext, _queryExecutorDispatcher);
        var action = async () => await outputSample.GetForChallengeAsync(challenge.Id, 3);
        
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Solution SQL not found for challengeId: {challenge.Id}");
    }

    [Fact]
    public async Task ShouldThrowInvalidOperationException_WhenSolutionSqlFails()
    {
        var challenge = new ChallengeBuilder()
            .WithSolution("SELECT * FROM non_existing_table", DatabaseProvider.SqlServer)
            .Build();
        
        _dbContext.Challenges.Add(challenge);
        await _dbContext.SaveChangesAsync();
        
        var outputSample = new SampleOutputProvider(_cache, _dbContext, _queryExecutorDispatcher);
        var action = async () => await outputSample.GetForChallengeAsync(challenge.Id, 3);
        
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Failed to execute solution query for challengeId: {challenge.Id} {QueryErrorType.Unknown}");
    }

    [Fact]
    public async Task ShouldReturnErrorNullValue_WhenChallengeDoesNotExist()
    {
        var outputSample = new SampleOutputProvider(_cache, _dbContext, _queryExecutorDispatcher);
        var result = await outputSample.GetForChallengeAsync(Guid.NewGuid(), 3);

        result.Failed.Should().BeTrue();
        result.Error.Code.Should().Be("Error.NullValue");
    }
}