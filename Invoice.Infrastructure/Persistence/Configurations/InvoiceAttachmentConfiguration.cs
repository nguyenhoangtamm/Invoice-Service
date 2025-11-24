using Invoice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoice.Infrastructure.Persistence.Configurations;

public class InvoiceAttachmentConfiguration : IEntityTypeConfiguration<InvoiceAttachment>
{
    public void Configure(EntityTypeBuilder<InvoiceAttachment> b)
    {
        b.ToTable("InvoiceAttachments");
        b.HasKey(x => x.Id);

        b.Property(x => x.FileName).HasMaxLength(500).IsRequired();
        b.Property(x => x.ContentType).HasMaxLength(200).IsRequired();
        b.Property(x => x.Path).HasMaxLength(1000).IsRequired();
        b.Property(x => x.Size).HasPrecision(18,0);

        b.HasOne(x => x.Invoice)
            .WithMany(i => i.Attachments)
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
