using Invoice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoice.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(512);
        
        builder.Property(x => x.ExpiresAt)
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.Property(x => x.DeviceInfo)
            .HasMaxLength(100);
        
        builder.Property(x => x.IpAddress)
            .HasMaxLength(45);
        
        // Relationships
        builder.HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Indexes
        builder.HasIndex(x => x.TokenHash)
            .IsUnique();
        
        builder.HasIndex(x => x.UserId);
        
        builder.HasIndex(x => x.ExpiresAt);
        
        builder.HasIndex(x => new { x.UserId, x.IsRevoked });
    }
}

