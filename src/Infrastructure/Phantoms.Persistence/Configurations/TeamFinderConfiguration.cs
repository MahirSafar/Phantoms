using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Phantoms.Domain.Entities;

namespace Phantoms.Persistence.Configurations;

public class TeamFinderConfiguration : IEntityTypeConfiguration<TeamFinder>
{
    public void Configure(EntityTypeBuilder<TeamFinder> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(t => t.ProjectName).HasMaxLength(200);
        builder.Property(t => t.RequiredSkills).HasMaxLength(500);
        
        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}