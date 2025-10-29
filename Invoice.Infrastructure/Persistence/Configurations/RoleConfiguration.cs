using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Invoice.Domain.Entities;

namespace Invoice.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Identity already configures the table name as "AspNetRoles", we can override it
        builder.ToTable("Roles");

        // Configure additional properties (Identity already handles Name)
        builder.Property(e => e.Description)
            .HasMaxLength(500);

        // Configure audit properties
        builder.Property(e => e.CreatedBy)
            .HasMaxLength(100);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(e => e.CreatedDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);
    }
}


