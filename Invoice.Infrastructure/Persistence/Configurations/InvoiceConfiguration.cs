using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoice.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice.Domain.Entities.Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice.Domain.Entities.Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.Property(i => i.InvoiceNumber).HasMaxLength(200).IsRequired();
        builder.Property(i => i.FormNumber).HasMaxLength(100);
        builder.Property(i => i.Serial).HasMaxLength(50);

        builder.Property(i => i.SellerName).HasMaxLength(250);
        builder.Property(i => i.SellerTaxId).HasMaxLength(100);
        builder.Property(i => i.SellerAddress).HasMaxLength(500);
        builder.Property(i => i.SellerPhone).HasMaxLength(50);
        builder.Property(i => i.SellerEmail).HasMaxLength(250);

        builder.Property(i => i.CustomerName).HasMaxLength(250);
        builder.Property(i => i.CustomerTaxId).HasMaxLength(100);
        builder.Property(i => i.CustomerAddress).HasMaxLength(500);
        builder.Property(i => i.CustomerPhone).HasMaxLength(50);
        builder.Property(i => i.CustomerEmail).HasMaxLength(250);

        builder.Property(i => i.Status).HasMaxLength(50);
        builder.Property(i => i.Currency).HasMaxLength(10);
        builder.Property(i => i.Note).HasMaxLength(2000);

        builder.HasOne<Invoice.Domain.Entities.Organization>(i => i.TenantOrganization)
               .WithMany(o => o.Invoices)
               .HasForeignKey(i => i.TenantOrganizationId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Invoice.Domain.Entities.User>(i => i.IssuedByUser)
               .WithMany()
               .HasForeignKey(i => i.IssuedByUserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Invoice.Domain.Entities.InvoiceBatch>(i => i.Batch)
               .WithMany(b => b.Invoices)
               .HasForeignKey(i => i.BatchId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany<Invoice.Domain.Entities.InvoiceLine>(i => i.Lines)
               .WithOne(l => l.Invoice)
               .HasForeignKey(l => l.InvoiceId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(i => i.CreatedBy).HasMaxLength(100);
        builder.Property(i => i.UpdatedBy).HasMaxLength(100);
        builder.Property(i => i.IsDeleted).HasDefaultValue(false);

        // Numeric precision configuration
        builder.Property(i => i.Subtotal).HasPrecision(18, 2);
        builder.Property(i => i.TaxAmount).HasPrecision(18, 2);
        builder.Property(i => i.DiscountAmount).HasPrecision(18, 2);
        builder.Property(i => i.TotalAmount).HasPrecision(18, 2);
    }
}
