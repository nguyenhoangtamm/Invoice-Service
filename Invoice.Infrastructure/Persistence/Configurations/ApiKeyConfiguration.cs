using Invoice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoice.Infrastructure.Persistence.Configurations;

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> b)
    {
        b.ToTable("ApiKeys");

        b.HasKey(x => x.Id);

        b.Property(x => x.KeyHash)
            .IsRequired()
            .HasMaxLength(256);

        b.Property(x => x.Name)
            .HasMaxLength(200);

        b.HasOne(x => x.Organization)
            .WithMany(o => o.ApiKeys)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
