using Invoice.Domain.Interfaces;
namespace Invoice.Domain.Entities.Base;

public abstract class BaseAuditableEntity : BaseEntity, IAuditableEntity
{
    public int? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }

    // FK references to Users.Id per ERD
    public int? CreatedById { get; set; }
    public int? UpdatedById { get; set; }
}

