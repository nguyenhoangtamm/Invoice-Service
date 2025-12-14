using System.ComponentModel.DataAnnotations;

namespace Invoice.Domain.DTOs.Auth;

public class LoginRequest
{
    [Required]
    public string UsernameOrEmail { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MinLength(3)]
    public string UserName { get; set; } = string.Empty;

    // Profile fields
    [Required]
    [MinLength(2)]
    [MaxLength(200)]
    public string Fullname { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Gender { get; set; }

    public DateTime? BirthDate { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(1000)]
    public string? Bio { get; set; }

    [Phone]
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }
}

public class AuthResponse
{
    public bool success { get; set; }
    public AuthData Data { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

public class AuthData
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public UserInfo User { get; set; } = new();
}

public class UserInfo
{
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class LogoutRequest
{
    [Required]
    public string UserId { get; set; } = string.Empty;
}

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}

