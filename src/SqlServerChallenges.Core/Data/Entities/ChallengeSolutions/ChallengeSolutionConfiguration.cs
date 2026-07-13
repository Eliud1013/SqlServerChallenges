using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlServerChallenges.Core.Data.Entities;
using SqlServerChallenges.Core.Services.SqlExecutor;

namespace SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;

public class ChallengeSolutionConfiguration : IEntityTypeConfiguration<ChallengeSolution>
{
    public void Configure(EntityTypeBuilder<ChallengeSolution> builder)
    {
        builder.ToTable("ChallengeSolutions", "Challenges");

        builder.HasKey(x => x.ChallengeId);

        builder.Property(x => x.DatabaseProvider)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.SolutionSql)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.HasOne<Challenge>(x => x.Challenge)
            .WithMany(c => c.Solutions)
            .HasForeignKey(x => x.ChallengeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}