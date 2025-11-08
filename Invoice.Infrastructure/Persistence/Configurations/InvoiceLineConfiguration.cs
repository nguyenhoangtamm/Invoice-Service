using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoice.Infrastructure.Persistence.Configurations;

public class InvoiceLineConfiguration : IEntityTypeConfiguration<Invoice.Domain.Entities.InvoiceLine>
{
    public void Configure(EntityTypeBuilder<Invoice.Domain.Entities.InvoiceLine> builder)
    {
        builder.ToTable("InvoiceLines");

        builder.Property(l => l.LineNumber).IsRequired();
        builder.Property(l => l.Description).HasMaxLength(1000);
        builder.Property(l => l.Unit).HasMaxLength(50);

        builder.Property(l => l.Quantity).HasPrecision(18, 2);
        builder.Property(l => l.UnitPrice).HasPrecision(18, 2);
        builder.Property(l => l.Discount).HasPrecision(18, 2);
        builder.Property(l => l.TaxRate).HasPrecision(5, 2);
        builder.Property(l => l.TaxAmount).HasPrecision(18, 2);
        builder.Property(l => l.LineTotal).HasPrecision(18, 2);

        builder.HasOne<Invoice.Domain.Entities.Invoice>(l => l.Invoice)
               .WithMany(i => i.Lines)
               .HasForeignKey(l => l.InvoiceId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(l => l.CreatedBy).HasMaxLength(100);
        builder.Property(l => l.UpdatedBy).HasMaxLength(100);
        builder.Property(l => l.IsDeleted).HasDefaultValue(false);
    }
}
