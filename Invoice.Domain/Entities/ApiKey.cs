using Invoice.Domain.Entities.Base;

namespace Invoice.Domain.Entities;

public class ApiKey : BaseAuditableEntity
{
    public string KeyHash { get; set; } = string.Empty;
    public string? Name { get; set; }

    public int OrganizationId { get; set; }
    public virtual Organization Organization { get; set; } = null!;

    public bool Active { get; set; } = true;
    public DateTime? RevokedAt { get; set; }
}
