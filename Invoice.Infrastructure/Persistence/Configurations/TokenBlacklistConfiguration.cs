using Invoice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoice.Infrastructure.Persistence.Configurations;

public class TokenBlacklistConfiguration : IEntityTypeConfiguration<TokenBlacklist>
{
    public void Configure(EntityTypeBuilder<TokenBlacklist> builder)
    {
        builder.ToTable("TokenBlacklists");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(512);
        
        builder.Property(x => x.ExpiresAt)
            .IsRequired();
        
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        
        builder.Property(x => x.TokenType)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("access");
        
        builder.Property(x => x.Reason)
            .HasMaxLength(100);
        
        // Relationships
        builder.HasOne(x => x.User)
            .WithMany(x => x.TokenBlacklists)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // Indexes
        builder.HasIndex(x => x.TokenHash)
            .IsUnique();
        
        builder.HasIndex(x => x.ExpiresAt);
        
        builder.HasIndex(x => new { x.TokenType, x.ExpiresAt });
    }
}

