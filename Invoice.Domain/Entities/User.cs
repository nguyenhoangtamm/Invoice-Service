using Invoice.Domain.Enums;
using Invoice.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Invoice.Domain.Entities;

public class User : IdentityUser<int>, IAuditableEntity
{
    public int RoleId { get; set; }
    public string? FullName { get; set; }
    public UserStatus? Status { get; set; }

    public string? Phone { get; set; }

    // Audit fields from IAuditableEntity
    public int? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }

    // FK ids to users for audits
    public int? CreatedById { get; set; }
    public int? UpdatedById { get; set; }

    public virtual Role Role { get; set; } = null!;

    // JWT-related navigation properties
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public virtual ICollection<TokenBlacklist> TokenBlacklists { get; set; } = new List<TokenBlacklist>();

    // Profile navigation
    public virtual Profile? Profile { get; set; }

    public virtual ICollection<Organization>? Organizations { get; set; } = new List<Organization>();

    public virtual ICollection<Invoice> IssuedInvoices { get; set; } = new List<Invoice>();
    
    // Reports submitted by this user
    public virtual ICollection<InvoiceReport> InvoiceReports { get; set; } = new List<InvoiceReport>();
}



