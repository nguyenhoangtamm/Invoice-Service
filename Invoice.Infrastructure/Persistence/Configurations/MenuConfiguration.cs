using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Invoice.Domain.Entities;

namespace Invoice.Infrastructure.Persistence.Configurations;

public class MenuConfiguration : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("Menus");

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Path)
            .HasMaxLength(200);

        builder.Property(e => e.Icon)
            .HasMaxLength(100);

        // Self-referencing relationship
        builder.HasOne(e => e.Parent)
            .WithMany(m => m.Children)
            .HasForeignKey(e => e.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Apply base configuration
        new BaseAuditableEntityConfiguration<Menu>().Configure(builder);
    }
}


