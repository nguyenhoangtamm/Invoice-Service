using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Invoice.Domain.Entities;

namespace Invoice.Infrastructure.Persistence.Configurations;

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.ToTable("ApiKeys");

        builder.Property(a => a.KeyHash).HasMaxLength(500).IsRequired();
        builder.Property(a => a.Name).HasMaxLength(250);
        builder.Property(a => a.Active).HasDefaultValue(true);
        builder.Property(a => a.RevokedAt);

        builder.Property(a => a.CreatedBy).HasMaxLength(100);
        builder.Property(a => a.UpdatedBy).HasMaxLength(100);
        builder.Property(a => a.IsDeleted).HasDefaultValue(false);

        builder.HasOne(a => a.Organization)
               .WithMany(o => o.ApiKeys)
               .HasForeignKey(a => a.OrganizationId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
