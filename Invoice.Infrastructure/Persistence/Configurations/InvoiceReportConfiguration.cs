using Invoice.Domain.Entities;
using Invoice.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoice.Infrastructure.Persistence.Configurations;

public class InvoiceReportConfiguration : IEntityTypeConfiguration<InvoiceReport>
{
    public void Configure(EntityTypeBuilder<InvoiceReport> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Reason)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(x => x.Status)
            .IsRequired();

        // Foreign key to Invoice
        builder.HasOne(x => x.Invoice)
            .WithMany(i => i.Reports)
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Foreign key to User (ReportedByUser)
        builder.HasOne(x => x.ReportedByUser)
            .WithMany(u => u.InvoiceReports)
            .HasForeignKey(x => x.ReportedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("InvoiceReports");
    }
}
