using Invoice.Domain.Entities.Base;

namespace Invoice.Domain.Entities;

public class Profile : BaseAuditableEntity
{
    public int UserId { get; set; }
    public string Fullname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;

    // Navigation property
    public virtual User User { get; set; } = null!;
}