using Invoice.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Invoice.Infrastructure.Persistence.Configurations;

public class InvoiceBatchConfiguration : IEntityTypeConfiguration<InvoiceBatch>
{
    public void Configure(EntityTypeBuilder<InvoiceBatch> b)
    {
        b.ToTable("InvoiceBatches");

        b.HasKey(x => x.Id);

        b.Property(x => x.BatchId)
            .IsRequired()
            .HasMaxLength(100);

        b.Property(x => x.MerkleRoot).HasMaxLength(256);
        b.Property(x => x.BatchCid).HasMaxLength(200);
        b.Property(x => x.TxHash).HasMaxLength(200);

        b.HasIndex(x => x.BatchId).IsUnique();
    }
}
