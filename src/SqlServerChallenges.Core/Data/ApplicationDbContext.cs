using Microsoft.EntityFrameworkCore;
using SqlServerChallenges.Core.Data.Entities;

namespace SqlServerChallenges.Core.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Challenge>  Challenges { get; init; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}