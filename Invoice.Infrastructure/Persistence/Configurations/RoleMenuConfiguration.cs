using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Invoice.Domain.Entities;

namespace Invoice.Infrastructure.Persistence.Configurations;

public class RoleMenuConfiguration : IEntityTypeConfiguration<RoleMenu>
{
    public void Configure(EntityTypeBuilder<RoleMenu> builder)
    {
        builder.ToTable("RoleMenus");

        // Composite unique index
        builder.HasIndex(e => new { e.RoleId, e.MenuId })
            .IsUnique();

        // Relationships
        builder.HasOne(e => e.Role)
            .WithMany(r => r.RoleMenus)
            .HasForeignKey(e => e.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Menu)
            .WithMany(m => m.RoleMenus)
            .HasForeignKey(e => e.MenuId)
            .OnDelete(DeleteBehavior.Cascade);

        // Apply base configuration
        new BaseAuditableEntityConfiguration<RoleMenu>().Configure(builder);
    }
}


