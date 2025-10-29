using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Invoice.Domain.Entities;

namespace Invoice.Infrastructure.Persistence.Configurations;

public class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.ToTable("Sections");

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Content)
            .HasMaxLength(5000);

        // Apply base configuration
        new BaseAuditableEntityConfiguration<Section>().Configure(builder);
    }
}


