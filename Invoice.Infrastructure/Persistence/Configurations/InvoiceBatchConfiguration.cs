using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Invoice.Domain.Entities;

namespace Invoice.Infrastructure.Persistence.Configurations;

public class InvoiceBatchConfiguration : IEntityTypeConfiguration<InvoiceBatch>
{
    public void Configure(EntityTypeBuilder<InvoiceBatch> builder)
    {
        builder.ToTable("InvoiceBatches");

        builder.Property(b => b.BatchId).HasMaxLength(200);
        builder.Property(b => b.MerkleRoot).HasMaxLength(500);
        builder.Property(b => b.BatchCid).HasMaxLength(500);
        builder.Property(b => b.Count);

        builder.Property(b => b.CreatedBy).HasMaxLength(100);
        builder.Property(b => b.UpdatedBy).HasMaxLength(100);
        builder.Property(b => b.IsDeleted).HasDefaultValue(false);

        builder.HasMany(b => b.Invoices)
               .WithOne(i => i.Batch)
               .HasForeignKey(i => i.BatchId)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
