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

        builder.Property(e => e.FullName)
            .HasMaxLength(200);

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

        builder.HasMany(e => e.Organizations)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // 1 user -> many issued invoices
        builder.HasMany(e => e.IssuedInvoices)
            .WithOne(inv => inv.IssuedByUser)
            .HasForeignKey(inv => inv.IssuedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(e => e.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.TokenBlacklists)
            .WithOne(tb => tb.User)
            .HasForeignKey(tb => tb.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}


