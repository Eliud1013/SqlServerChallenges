using Microsoft.EntityFrameworkCore;
using SqlServerChallenges.Core.Data;
using Testcontainers.MsSql;
using Xunit;

namespace SqlServerChallenges.Core.Tests.Common;

public class SqlServerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _container;
    public string _connectionString = string.Empty; 

    public SqlServerFixture()
    {
        _container = new MsSqlBuilder(image: "mcr.microsoft.com/mssql/server:2022-latest")
            .Build();
    }

    public ApplicationDbContext CreateDbContext()
    {
        
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(_connectionString);

        return new ApplicationDbContext(options.Options);
    }

    public async Task ClearDatabaseAsync(ApplicationDbContext dbContext)
    {
        try
        {
            await dbContext.Challenges.ExecuteDeleteAsync();
            await dbContext.Categories.ExecuteDeleteAsync();
            await dbContext.Solutions.ExecuteDeleteAsync();
        }
        finally
        {
            await dbContext.DisposeAsync();
        }
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        _connectionString = _container.GetConnectionString();
        
        await using ApplicationDbContext dbContext = CreateDbContext();
        await dbContext.Database.MigrateAsync();
    }

    public Task DisposeAsync()
    {
        return _container.DisposeAsync().AsTask();
    }
}