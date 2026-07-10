using Microsoft.EntityFrameworkCore;
using SqlServerChallenges.Core.Data;
using Testcontainers.MsSql;
using Xunit;

namespace SqlServerChallenges.Core.IntegrationTests.Common;

public class SqlServerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container;

    public SqlServerFixture()
    {
        _container = new MsSqlBuilder(image: "mcr.microsoft.com/mssql/server:2022-latest")
            .Build();
    }

    public ApplicationDbContext CreateDbContext()
    {
        var connectionString = _container.GetConnectionString();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connectionString);

        return new ApplicationDbContext(options.Options);
    }

    public async Task ClearDatabaseAsync(ApplicationDbContext dbContext)
    {
        try
        {
            await dbContext.Challenges.ExecuteDeleteAsync();
            await dbContext.Categories.ExecuteDeleteAsync();
        }
        finally
        {
            await dbContext.DisposeAsync();
        }
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using ApplicationDbContext dbContext = CreateDbContext();
        await dbContext.Database.MigrateAsync();
    }

    public Task DisposeAsync()
    {
        return _container.DisposeAsync().AsTask();
    }
}