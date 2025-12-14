using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Invoice.Domain.Entities.Base;

namespace Invoice.Infrastructure.Persistence.Configurations;

public class BaseAuditableEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BaseAuditableEntity
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(e => e.CreatedBy);
        builder.Property(e => e.UpdatedBy);
        builder.Property(e => e.IsDeleted).HasDefaultValue(false);

        // Optionally configure auditing timestamps as required
        builder.Property(e => e.CreatedDate);
        builder.Property(e => e.UpdatedDate);
    }
}


