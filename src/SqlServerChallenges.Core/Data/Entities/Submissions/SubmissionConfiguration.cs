using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SqlServerChallenges.Core.Data.Entities.Submissions;

public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.ToTable("Submissions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ChallengeId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Passed)
            .IsRequired();

        builder.Property(x => x.SubmittedAtUtc)
            .IsRequired();
    }
}