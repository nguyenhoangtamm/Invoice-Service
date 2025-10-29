using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;

namespace Invoice.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Identity already configures the table name as "AspNetUsers", we can override it
        builder.ToTable("Users");

        // Configure additional properties (Identity already handles UserName, Email, PasswordHash, etc.)
        builder.Property(e => e.Status)
            .IsRequired()
            .HasDefaultValue(UserStatus.Active);

        builder.Property(e => e.RoleId)
            .IsRequired();

        // Configure audit properties
        builder.Property(e => e.CreatedBy)
            .HasMaxLength(100);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(e => e.CreatedDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(e => e.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(e => e.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}


