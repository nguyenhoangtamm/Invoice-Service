using Invoice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoice.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice.Domain.Entities.Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice.Domain.Entities.Invoice> b)
    {
        b.ToTable("Invoices");

        b.HasKey(x => x.Id);

        // Basic
        b.Property(x => x.InvoiceNumber).HasMaxLength(50);
        b.Property(x => x.FormNumber).HasMaxLength(50);
        b.Property(x => x.Serial).HasMaxLength(50);

        // Seller
        b.Property(x => x.SellerName).HasMaxLength(200);
        b.Property(x => x.SellerTaxId).HasMaxLength(50);
        b.Property(x => x.SellerAddress).HasMaxLength(500);
        b.Property(x => x.SellerPhone).HasMaxLength(30);
        b.Property(x => x.SellerEmail).HasMaxLength(200);

        // Customer
        b.Property(x => x.CustomerName).HasMaxLength(200);
        b.Property(x => x.CustomerTaxId).HasMaxLength(50);
        b.Property(x => x.CustomerAddress).HasMaxLength(500);
        b.Property(x => x.CustomerPhone).HasMaxLength(30);
        b.Property(x => x.CustomerEmail).HasMaxLength(200);

        b.Property(x => x.Currency).HasMaxLength(10);
        b.Property(x => x.Note).HasMaxLength(4000);

        // Hash / Cid
        b.Property(x => x.ImmutableHash).HasMaxLength(256);
        b.Property(x => x.Cid).HasMaxLength(200);
        b.Property(x => x.CidHash).HasMaxLength(256);
        b.Property(x => x.MerkleProof).HasColumnType("text");

        // Decimal precision
        b.Property(x => x.SubTotal).HasPrecision(18, 2);
        b.Property(x => x.TaxAmount).HasPrecision(18, 2);
        b.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        b.Property(x => x.TotalAmount).HasPrecision(18, 2);

        // Relations
        b.HasOne(x => x.Organization)
            .WithMany(o => o.Invoices)
            .HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.IssuedByUser)
            .WithMany(u => u.IssuedInvoices)
            .HasForeignKey(x => x.IssuedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(x => x.Batch)
            .WithMany(bh => bh.Invoices)
            .HasForeignKey(x => x.BatchId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
