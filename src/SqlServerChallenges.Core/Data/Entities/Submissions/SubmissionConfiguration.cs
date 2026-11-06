using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlServerChallenges.Core.Data.Entities.QueryPlans;

namespace SqlServerChallenges.Core.Data.Entities.Submissions;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("Submissions", "Submissions");
        
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.UserId);

        builder.Property(x => x.ChallengeId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Passed)
            .IsRequired();
        
        builder.Property(x => x.Outcome)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.SubmittedAtUtc)
            .IsRequired();

        builder.HasOne(x => x.Plan)
            .WithOne()
            .HasForeignKey<SubmissionPlan>(x => x.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}