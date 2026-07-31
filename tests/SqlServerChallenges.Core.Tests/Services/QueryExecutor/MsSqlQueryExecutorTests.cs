using FluentAssertions;
using Microsoft.Data.SqlClient;
using SqlServerChallenges.Core.Data.Entities;
using SqlServerChallenges.Core.Data.Entities.Categories;
using SqlServerChallenges.Core.Data.Entities.Challenges;
using SqlServerChallenges.Core.Services.SqlExecutor;
using SqlServerChallenges.Core.Tests.Common;
using Xunit;

namespace SqlServerChallenges.Core.Tests.Services.QueryExecutor;

public class MsSqlQueryExecutorTests : BaseIntegrationTest
{
    public MsSqlQueryExecutorTests(SqlServerFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task ShouldReturnQueryResult_WhenQueryIsValid()
    {
        await using var connection = new SqlConnection(_fixture._connectionString);

        var executor = new MsSqlQueryExecutor(connection);
        string query = "SELECT 1";

        var result = await executor.ExecuteQueryAsync(query);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Columns.Count.Should().Be(1);
        result.Rows.Count.Should().Be(1);
    }

    [Fact]
    public async Task ShouldReturnTimeoutError_WhenQueryExceedsTimeout()
    {
        await using var connection = new SqlConnection(_fixture._connectionString);

        var executor = new MsSqlQueryExecutor(connection);
        string query = "WAITFOR DELAY '00:00:30';";

        var result = await executor.ExecuteQueryAsync(query);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(QueryErrorType.QueryTimeout);
    }

    [Fact]
    public async Task ShouldReturnData_WhenQueryingExistingData()
    {
        _dbContext.Challenges.Add(new Challenge
        {
            Id = Guid.NewGuid(),
            Title = "Basic SELECT",
            TaskDescription = "Write a SELECT query",
            Difficulty = ChallengeDifficulty.Easy,
            RequiresOrdering = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Category = new Category { Name = "Basic Queries" }
        });

        await _dbContext.SaveChangesAsync();

        await using var connection = new SqlConnection(_fixture._connectionString);
        var executor = new MsSqlQueryExecutor(connection);

        string query = "SELECT * FROM Challenges.Challenges";
        var result = await executor.ExecuteQueryAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Rows.Count.Should().Be(1);
        var row = result.Rows[0];
        var columns = result.Columns;
        
        row["Title"].Should().Be("Basic SELECT");
        row["TaskDescription"].Should().Be("Write a SELECT query");
        row["Difficulty"].Should().Be(nameof(ChallengeDifficulty.Easy));
        columns.Should().Contain("Title");
        columns.Should().Contain("TaskDescription");
        columns.Should().Contain("Difficulty");
        columns.Count.Should().Be(8);
    }

    [Fact]
    public async Task ShouldRespectMaxBatchRows_WhenExceedingLimit()
    {
        await using var connection = new SqlConnection(_fixture._connectionString);
        var executor = new MsSqlQueryExecutor(connection);

        string query = "SELECT value FROM generate_series(1,100);";
        var result = await executor.ExecuteQueryAsync(query);

        result.IsSuccess.Should().BeTrue();
        result.Rows.Count.Should().Be(50);
    }


    [Fact]
    public async Task ShouldReturnPermissionDeniedError_WhenUserLacksTableAccess()
    {
        await using var setupConnection = new SqlConnection(_fixture._connectionString);
        await setupConnection.OpenAsync();
        
        var setupCommand = new SqlCommand(@"
            CREATE TABLE TestTable(Id INT PRIMARY KEY);
            CREATE USER test_user
            WITHOUT LOGIN;", setupConnection);

        await setupCommand.ExecuteNonQueryAsync();

        await using var connection = new SqlConnection(_fixture._connectionString);
        var executor = new MsSqlQueryExecutor(connection);

        string query = @"
            EXECUTE AS USER = 'test_user';
            SELECT * FROM TestTable;";

        var result = await executor.ExecuteQueryAsync(query);

        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(QueryErrorType.PermissionDenied);
    }
}
