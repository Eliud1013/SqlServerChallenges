using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SqlServerChallenges.Core.Data.Entities.Categories;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories", "Challenges");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasMany(c => c.Challenges)
            .WithMany(c => c.Categories);
    }
}