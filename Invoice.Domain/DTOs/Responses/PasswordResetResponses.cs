namespace Invoice.Domain.DTOs.Responses;

public class PasswordResetResponse
{
    public string Message { get; set; } = string.Empty;
}

public class ForgotPasswordResponse
{
    public string Message { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
