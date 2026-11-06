using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;

namespace SqlServerChallenges.Core.Data.Entities.QueryPlans;

public class SubmissionPlanConfiguration : IEntityTypeConfiguration<SubmissionPlan>
{
    public void Configure(EntityTypeBuilder<SubmissionPlan> builder)
    {
        builder.ToTable("SubmissionPlans", "Submissions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasDefaultValueSql("newsequentialid()");

        builder.Property(x => x.SubmissionId)
            .IsRequired();

        builder.Property(x => x.Provider)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.PhysicalOperation)
            .HasMaxLength(128);

        builder.Property(x => x.LogicalOperation)
            .HasMaxLength(128);

        builder.Property(x => x.EstimateIO);

        builder.Property(x => x.EstimateCPU);
    }
}
