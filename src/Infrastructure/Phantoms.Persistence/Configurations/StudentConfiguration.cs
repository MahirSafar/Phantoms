using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phantoms.Domain.Entities;

namespace Phantoms.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.University).HasMaxLength(200);
        builder.Property(s => s.Faculty).HasMaxLength(200);
        builder.Property(s => s.Specialty).HasMaxLength(200);
        builder.Property(s => s.Bio).HasMaxLength(1000);

        builder.HasOne(s => s.AppUser)
            .WithOne()
            .HasForeignKey<Student>(s => s.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.LostFounds)
            .WithOne(l => l.Student)
            .HasForeignKey(l => l.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.TeamFinders)
            .WithOne(t => t.Student)
            .HasForeignKey(t => t.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}