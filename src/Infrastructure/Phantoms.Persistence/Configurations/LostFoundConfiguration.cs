using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phantoms.Domain.Entities;

namespace Phantoms.Persistence.Configurations;

public class LostFoundConfiguration : IEntityTypeConfiguration<LostFound>
{
    public void Configure(EntityTypeBuilder<LostFound> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(l => l.Location).HasMaxLength(300);
        builder.Property(l => l.ImageUrl).HasMaxLength(500);

        builder.HasQueryFilter(l => !l.IsDeleted);
    }
}