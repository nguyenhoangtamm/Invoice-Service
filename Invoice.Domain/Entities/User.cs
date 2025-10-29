using Invoice.Domain.Enums;
using Invoice.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Invoice.Domain.Entities;

public class User : IdentityUser<int>, IAuditableEntity
{
    public int RoleId { get; set; }
    public UserStatus? Status { get; set; }

    // Audit fields from IAuditableEntity
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsDeleted { get; set; }

    public virtual Role Role { get; set; } = null!;
    
    // JWT-related navigation properties
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public virtual ICollection<TokenBlacklist> TokenBlacklists { get; set; } = new List<TokenBlacklist>();

    // Profile navigation
    public virtual Profile? Profile { get; set; }
}


