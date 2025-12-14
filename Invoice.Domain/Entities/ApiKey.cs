using Invoice.Domain.Entities.Base;

namespace Invoice.Domain.Entities;

public class ApiKey : BaseAuditableEntity
{
    public string KeyHash { get; set; } = default!;
    public string? Name { get; set; }
    public bool Active { get; set; } = true;
    public DateTime? RevokedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = default!;
}
