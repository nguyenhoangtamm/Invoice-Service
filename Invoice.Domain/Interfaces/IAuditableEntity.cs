using Invoice.Domain.Entities.Base;
namespace Invoice.Domain.Interfaces;

public interface IAuditableEntity : IEntity
{
    int? CreatedBy { get; set; }
    DateTime? CreatedDate { get; set; }
    int? UpdatedBy { get; set; }
    DateTime? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }

    // FK references to Users.Id per ERD
    public int? CreatedById { get; set; }
    public int? UpdatedById { get; set; }
}

