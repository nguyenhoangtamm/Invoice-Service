using Invoice.Domain.Interfaces;
namespace Invoice.Domain.Entities.Base;

public abstract class BaseAuditableEntity : BaseEntity, IAuditableEntity
{
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }
}

