using Invoice.Application.Services;
using Invoice.Domain.DTOs.Requests;
using Invoice.Domain.DTOs.Responses;
using Invoice.Domain.Shares;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Controllers;

public class PasswordResetController : ApiControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public PasswordResetController(ILogger<PasswordResetController> logger, IAuthService authService, IConfiguration configuration) 
        : base(logger)
    {
        _authService = authService;
        _configuration = configuration;
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<Result<ForgotPasswordResponse>>> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(Result<ForgotPasswordResponse>.Failure("Email is required"));
            }

            LogInformation($"Forgot password request for email: {request.Email}");

            // Get the reset link base URL from configuration
            var resetLinkBase = _configuration["AppSettings:ResetPasswordUrl"] ?? "http://localhost:3000/reset-password";

            var (success, message) = await _authService.ForgotPasswordAsync(request.Email, resetLinkBase);

            var response = new ForgotPasswordResponse
            {
                Message = message,
                Email = request.Email
            };

            if (success)
            {
                return Ok(Result<ForgotPasswordResponse>.Success(response, message));
            }

            return BadRequest(Result<ForgotPasswordResponse>.Failure(message));
        }
        catch (Exception ex)
        {
            LogError("Error processing forgot password", ex);
            return StatusCode(500, Result<ForgotPasswordResponse>.Failure("An error occurred while processing your request"));
        }
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<Result<PasswordResetResponse>>> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest(Result<PasswordResetResponse>.Failure("Reset token is required"));
            }

            if (string.IsNullOrEmpty(request.NewPassword) || string.IsNullOrEmpty(request.ConfirmPassword))
            {
                return BadRequest(Result<PasswordResetResponse>.Failure("Password and confirm password are required"));
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                return BadRequest(Result<PasswordResetResponse>.Failure("Passwords do not match"));
            }

            if (request.NewPassword.Length < 6)
            {
                return BadRequest(Result<PasswordResetResponse>.Failure("Password must be at least 6 characters long"));
            }

            LogInformation("Processing password reset");

            var (success, message) = await _authService.ResetPasswordAsync(request.Token, request.NewPassword);

            var response = new PasswordResetResponse
            {
                Message = message
            };

            if (success)
            {
                return Ok(Result<PasswordResetResponse>.Success(response, message));
            }

            return BadRequest(Result<PasswordResetResponse>.Failure(message));
        }
        catch (Exception ex)
        {
            LogError("Error resetting password", ex);
            return StatusCode(500, Result<PasswordResetResponse>.Failure("An error occurred while resetting your password"));
        }
    }
}
