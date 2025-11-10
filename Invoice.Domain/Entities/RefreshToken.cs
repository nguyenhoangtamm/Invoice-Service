using System.ComponentModel.DataAnnotations;

namespace Invoice.Domain.Entities;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }

    public int? UserId { get; set; }

    [Required]
    [MaxLength(512)]
    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? RevokedAt { get; set; }

    [MaxLength(100)]
    public string? DeviceInfo { get; set; }

    [MaxLength(45)]
    public string? IpAddress { get; set; }

    // Navigation property
    public virtual User? User { get; set; } = null!;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;
}

