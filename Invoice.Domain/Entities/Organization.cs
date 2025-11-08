using Invoice.Domain.Entities.Base;

namespace Invoice.Domain.Entities;

public class Organization : BaseAuditableEntity
{
    public string OrganizationName { get; set; } = string.Empty;
    public string? OrganizationTaxId { get; set; }
    public string? OrganizationAddress { get; set; }
    public string? OrganizationPhone { get; set; }
    public string? OrganizationEmail { get; set; }
    public string? OrganizationBankAccount { get; set; }

    // Optional owner/user who created or owns this organization
    public int? UserId { get; set; }
    public virtual User? User { get; set; }

    // Navigation
    public virtual ICollection<ApiKey> ApiKeys { get; set; } = new List<ApiKey>();
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
