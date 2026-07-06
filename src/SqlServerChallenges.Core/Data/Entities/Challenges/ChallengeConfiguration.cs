using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlServerChallenges.Core.Data.Entities;
using SqlServerChallenges.Core.Data.Entities.Categories;

namespace SqlServerChallenges.Core.Data.Configurations;

public class ChallengeConfiguration : IEntityTypeConfiguration<Challenge>
{
    public void Configure(EntityTypeBuilder<Challenge> builder)
    {
        builder.ToTable("Challenges", "Challenges");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasDefaultValueSql("newsequentialid()");

        builder.Property(x => x.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.TaskDescription)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.SolutionQuery)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("getutcdate()");

        builder.Property(x => x.UpdatedAt)
            .HasDefaultValueSql("getutcdate()");

        builder.HasMany(c => c.Categories)
            .WithMany(c => c.Challenges)
            .UsingEntity<Dictionary<string, object>>(
                "ChallengeCategory",
                j => j.HasOne<Category>()
                    .WithMany()
                    .HasForeignKey("CategoryId"),
                j => j.HasOne<Challenge>()
                    .WithMany()
                    .HasForeignKey("ChallengeId"),
                j =>
                {
                    j.ToTable("ChallengeCategories", "Challenges");
                    j.HasKey("ChallengeId", "CategoryId");
                });
        
    }
}