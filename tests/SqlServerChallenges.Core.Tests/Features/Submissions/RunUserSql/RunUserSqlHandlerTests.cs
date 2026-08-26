using System.Data;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;
using SqlServerChallenges.Core.Features.Submissions.RunUserSql;
using SqlServerChallenges.Core.Services;
using SqlServerChallenges.Core.Services.QueryExecutor;
using SqlServerChallenges.Core.Tests.Common;
using Xunit;

namespace SqlServerChallenges.Core.Tests.Features.Submissions.RunUserSql;

public class RunUserSqlHandlerTests : BaseIntegrationTest
{
    public RunUserSqlHandlerTests(SqlServerFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task ShouldReturnChallengeNotFoundError_WhenChallengeDoesNotExist()
    {
        var queryExecutorMock = new Mock<IQueryExecutor>();

        queryExecutorMock.Setup(x => x.ExecuteQueryAsync(It.IsAny<string>()))
            .ReturnsAsync(new DataTable());
        
        var executorDispatcher = new QueryExecutorDispatcher(
            new[] { queryExecutorMock.Object });

        var syntaxDispatcher = new SyntaxCheckerDispatcher(
            new[] { new MsSqlQuerySyntaxChecker() });
        
        var handler = new RunUserSqlHandler(
            _dbContext,
            syntaxDispatcher,
            executorDispatcher,
            NullLogger<RunUserSqlHandler>.Instance,
            new MemoryCache(new MemoryCacheOptions()));

        var command = new RunUserSqlCommand(Guid.NewGuid(), "SELECT 1", DatabaseProvider.SqlServer);
        var result = await handler.Handle(command, CancellationToken.None);

        queryExecutorMock.Verify(
            x => x.ExecuteQueryAsync(It.IsAny<string>()),
            Times.Never);
        
        result.Failed.Should().BeTrue();
        result.Error.Should().NotBeNull();
        result.Error.Code.Should().Be("Submission.ChallengeNotFound");
    }
}