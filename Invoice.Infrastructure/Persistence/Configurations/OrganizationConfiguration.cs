using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Invoice.Domain.Entities;

namespace Invoice.Infrastructure.Persistence.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations");

        builder.Property(o => o.OrganizationName).HasMaxLength(250).IsRequired();
        builder.Property(o => o.OrganizationTaxId).HasMaxLength(100);
        builder.Property(o => o.OrganizationAddress).HasMaxLength(500);
        builder.Property(o => o.OrganizationPhone).HasMaxLength(50);
        builder.Property(o => o.OrganizationEmail).HasMaxLength(250);
        builder.Property(o => o.OrganizationBankAccount).HasMaxLength(200);

        builder.Property(o => o.CreatedBy).HasMaxLength(100);
        builder.Property(o => o.UpdatedBy).HasMaxLength(100);
        builder.Property(o => o.IsDeleted).HasDefaultValue(false);

        builder.HasOne(o => o.User)
               .WithMany()
               .HasForeignKey(o => o.UserId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
