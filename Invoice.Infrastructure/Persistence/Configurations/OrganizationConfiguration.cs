using Invoice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoice.Infrastructure.Persistence.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> b)
    {
        b.ToTable("Organizations");

        b.HasKey(x => x.Id);

        b.Property(x => x.OrganizationName)
            .IsRequired()
            .HasMaxLength(200);

        b.Property(x => x.OrganizationTaxId)
            .HasMaxLength(50);

        b.Property(x => x.OrganizationAddress)
            .HasMaxLength(500);

        b.Property(x => x.OrganizationPhone)
            .HasMaxLength(30);

        b.Property(x => x.OrganizationEmail)
            .HasMaxLength(200);

        b.Property(x => x.OrganizationBankAccount)
            .HasMaxLength(100);

        // Owner
        b.HasOne(x => x.User)
            .WithMany(u => u.Organizations)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

