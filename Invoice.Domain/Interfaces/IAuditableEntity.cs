using Invoice.Domain.Entities.Base;
namespace Invoice.Domain.Interfaces;

public interface IAuditableEntity : IEntity
{
    string? CreatedBy { get; set; }
    DateTime? CreatedDate { get; set; }
    string? UpdatedBy { get; set; }
    DateTime? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }
}

