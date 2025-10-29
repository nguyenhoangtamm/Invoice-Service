using System.ComponentModel.DataAnnotations;

namespace Invoice.Domain.Entities;

public class TokenBlacklist
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(512)]
    public string TokenHash { get; set; } = string.Empty;
    
    public DateTime ExpiresAt { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [MaxLength(50)]
    public string TokenType { get; set; } = "access"; // "access" or "refresh"
    
    public int? UserId { get; set; }
    
    [MaxLength(100)]
    public string? Reason { get; set; }
    
    // Navigation property
    public virtual User? User { get; set; }
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}

