using SqlServerChallenges.Core.Data;
using Xunit;

namespace SqlServerChallenges.Core.Tests.Common;

public abstract class BaseIntegrationTest: IClassFixture<SqlServerFixture>, IAsyncLifetime
{
    protected readonly SqlServerFixture _fixture;
    protected readonly ApplicationDbContext _dbContext;
    
    public BaseIntegrationTest(SqlServerFixture fixture)
    {
        _fixture = fixture;
        _dbContext = _fixture.CreateDbContext();
    }
    
    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await _fixture.ClearDatabaseAsync(_dbContext);
}