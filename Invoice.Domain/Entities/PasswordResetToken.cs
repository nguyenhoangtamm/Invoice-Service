using Invoice.Domain.Entities.Base;

namespace Invoice.Domain.Entities;

public class PasswordResetToken : BaseAuditableEntity
{
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
