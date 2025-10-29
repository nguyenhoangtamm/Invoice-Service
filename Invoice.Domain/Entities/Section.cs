using Invoice.Domain.Entities.Base;

namespace Invoice.Domain.Entities;

public class Section : BaseAuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
}