using Invoice.Domain.Entities.Base;

namespace Invoice.Domain.Entities;

public class Organization : BaseAuditableEntity
{
    public string OrganizationName { get; set; } = default!;
    public string? OrganizationTaxId { get; set; }
    public string? OrganizationAddress { get; set; }
    public string? OrganizationPhone { get; set; }
    public string? OrganizationEmail { get; set; }
    public string? OrganizationBankAccount { get; set; }

    // Owner (User)
    public int? UserId { get; set; }
    public User? User { get; set; } = default!;

    // Navigation
    public ICollection<ApiKey> ApiKeys { get; set; } = new List<ApiKey>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
