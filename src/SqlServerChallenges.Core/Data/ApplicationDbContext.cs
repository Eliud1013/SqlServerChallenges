using Microsoft.EntityFrameworkCore;
using SqlServerChallenges.Core.Data.Entities;
using SqlServerChallenges.Core.Data.Entities.Categories;

namespace SqlServerChallenges.Core.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Challenge>  Challenges { get; init; }
    public DbSet<Category>  Categories { get; init; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}