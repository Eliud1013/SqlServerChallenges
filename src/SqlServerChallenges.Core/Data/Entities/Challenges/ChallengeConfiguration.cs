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
        
        builder.Property(x => x.Difficulty)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("getutcdate()");

        builder.Property(x => x.UpdatedAt)
            .HasDefaultValueSql("getutcdate()");

        builder.HasOne(x => x.Category)
            .WithMany(c => c.Challenges);
            
        builder.HasMany(c => c.Solutions)
            .WithOne(s => s.Challenge)
            .HasForeignKey(s => s.ChallengeId);
        
    }
}