using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SqlServerChallenges.Core.Data.Entities.ChallengeVote;

public class ChallengeVoteConfiguration : IEntityTypeConfiguration<ChallengeVote>
{
    public void Configure(EntityTypeBuilder<ChallengeVote> builder)
    {
        builder.ToTable("ChallengeVotes");
        builder.HasKey(x => new { x.ChallengeId, x.UserId });
        
        builder.Property(x => x.Type)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.VotedAtUtc)
            .IsRequired();
    }
}