using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phantoms.Domain.Entities;

namespace Phantoms.Persistence.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(4000);
        builder.Property(e => e.Location).HasMaxLength(250);
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(e => e.PublishedBy).HasMaxLength(100);

        builder.HasOne(e => e.Teacher)
            .WithMany()
            .HasForeignKey(e => e.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

public class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Title).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Content).IsRequired().HasMaxLength(4000);
        builder.Property(a => a.Category).HasConversion<string>().HasMaxLength(30);
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(a => a.PublishedBy).HasMaxLength(100);

        builder.HasOne(a => a.Teacher)
            .WithMany()
            .HasForeignKey(a => a.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
