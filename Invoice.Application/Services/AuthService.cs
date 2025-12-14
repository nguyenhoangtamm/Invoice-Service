using System.Security.Claims;
using Invoice.Domain.Common.Security;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;
using Invoice.Domain.Interfaces;
using Invoice.Domain.Interfaces.Repositories;
using Invoice.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Invoice.Application.Services;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string usernameOrEmail, string password, string? deviceInfo = null, string? ipAddress = null);
    Task<AuthResult> RegisterAsync(string email, string password, string username, string fullname, string? gender = null, DateTime? birthDate = null, string? address = null, string? bio = null, string? phoneNumber = null, string? avatarUrl = null);
    Task<AuthResult> RefreshTokenAsync(string refreshToken);
    Task<bool> LogoutAsync(string userId, string? accessToken = null);
    Task<User?> GetUserByIdAsync(string userId);
    Task<(bool Success, string Message)> ForgotPasswordAsync(string email, string resetLink);
    Task<(bool Success, string Message)> ResetPasswordAsync(string token, string newPassword);
}

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<Role> roleManager,
        IJwtService jwtService,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<AuthResult> LoginAsync(string usernameOrEmail, string password, string? deviceInfo = null, string? ipAddress = null)
    {
        try
        {
            User? user = null;

            // Tìm user bằng email trước
            if (IsValidEmail(usernameOrEmail))
            {
                user = await _userManager.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == usernameOrEmail);
            }

            // Nếu không tìm thấy bằng email hoặc input không phải email, thì tìm bằng username
            if (user == null)
            {
                user = await _userManager.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserName == usernameOrEmail);
            }

            if (user == null)
            {
                return AuthResult.Failed("Invalid username/email or password.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
            {
                return AuthResult.Failed("Invalid username/email or password.");
            }

            // Get role name
            var roleName = user.Role?.Name ?? "Unknown";

            // Get full name from user, otherwise use username
            var fullName = user.FullName ?? user.UserName ?? "Unknown";

            var accessToken = await GenerateJwtTokenAsync(user);
            var refreshToken = await GenerateAndStoreRefreshTokenAsync(user, deviceInfo, ipAddress);

            return AuthResult.Success(
                accessToken,
                refreshToken,
                user.Id.ToString(),
                user.UserName!,
                user.Email!,
                fullName,
                roleName
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for username/email: {UsernameOrEmail}", usernameOrEmail);
            return AuthResult.Failed("An error occurred during login.");
        }
    }

    public async Task<AuthResult> RegisterAsync(string email, string password, string username, string fullname, string? gender = null, DateTime? birthDate = null, string? address = null, string? bio = null, string? phoneNumber = null, string? avatarUrl = null)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return AuthResult.Failed("User with this email already exists.");
            }

            // Kiểm tra username đã tồn tại chưa
            var existingUserByName = await _userManager.FindByNameAsync(username);
            if (existingUserByName != null)
            {
                return AuthResult.Failed("User with this username already exists.");
            }

            // Automatically assign role ID 2 (Student/User role)
            const int defaultRoleId = 2;

            var user = new User
            {
                UserName = username,
                Email = email,
                RoleId = defaultRoleId,
                Status = UserStatus.Active,
                EmailConfirmed = true,
                FullName = fullname,
                Gender = gender,
                BirthDate = birthDate,
                Address = address,
                Bio = bio,
                Phone = phoneNumber,
                AvatarUrl = avatarUrl
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return AuthResult.Failed($"User creation failed: {errors}");
            }

            // Load user with role information
            user = await _userManager.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            // Get role information
            var roleName = user?.Role?.Name ?? "Unknown";

            return AuthResult.Success(
                null,
                null,
                user!.Id.ToString(),
                user.UserName!,
                user.Email!,
                fullname,
                roleName
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email: {Email}", email);
            return AuthResult.Failed("An error occurred during registration.");
        }
    }

    public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            // Validate refresh token format
            var principal = _jwtService.ValidateToken(refreshToken);
            if (principal == null)
            {
                return AuthResult.Failed("Invalid refresh token.");
            }

            // Kiểm tra xem có phải refresh token không
            var tokenType = principal.FindFirst("token_type")?.Value;
            if (tokenType != "refresh")
            {
                return AuthResult.Failed("Invalid token type.");
            }

            // Lấy user ID từ token
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return AuthResult.Failed("Invalid user ID in token.");
            }

            // Kiểm tra refresh token trong database
            var tokenHash = _jwtService.ComputeTokenHash(refreshToken);

            // Use JWT service to validate refresh token
            var isValidToken = await _jwtService.IsRefreshTokenValidAsync(tokenHash, userId);
            if (!isValidToken)
            {
                return AuthResult.Failed("Refresh token not found or expired.");
            }

            // Get stored token for device info
            var storedToken = await _refreshTokenRepository.GetByTokenHashAndUserIdAsync(tokenHash, userId);

            // Tìm user
            var user = await _userManager.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return AuthResult.Failed("User not found.");
            }

            // Revoke old refresh token (token rotation)
            await _jwtService.RevokeRefreshTokenAsync(tokenHash, userId);

            // Tạo token mới
            var newAccessToken = await GenerateJwtTokenAsync(user);
            var newRefreshToken = await GenerateAndStoreRefreshTokenAsync(user, storedToken?.DeviceInfo, storedToken?.IpAddress);

            // Get user info
            var roleName = user.Role?.Name ?? "Unknown";
            var fullName = user.FullName ?? user.UserName ?? "Unknown";

            return AuthResult.Success(
                newAccessToken,
                newRefreshToken,
                user.Id.ToString(),
                user.UserName!,
                user.Email!,
                fullName,
                roleName
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during refresh token");
            return AuthResult.Failed("An error occurred during token refresh.");
        }
    }

    public async Task<bool> LogoutAsync(string userId, string? accessToken = null)
    {
        try
        {
            if (int.TryParse(userId, out var userIdInt))
            {
                // Revoke all refresh tokens for the user
                await _jwtService.RevokeAllUserRefreshTokensAsync(userIdInt);

                // Blacklist the current access token if provided
                if (!string.IsNullOrEmpty(accessToken))
                {
                    await _jwtService.BlacklistTokenAsync(accessToken, "access", userIdInt, "User logout");
                }

                await _signInManager.SignOutAsync();
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout for user: {UserId}", userId);
            return false;
        }
    }

    public async Task<User?> GetUserByIdAsync(string userId)
    {
        try
        {
            if (int.TryParse(userId, out var id))
            {
                return await _userManager.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == id);
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
            return null;
        }
    }

    public async Task<(bool Success, string Message)> ForgotPasswordAsync(string email, string resetLink)
    {
        try
        {
            _logger.LogInformation($"Forgot password request for email: {email}");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Don't reveal if email exists - security best practice
                return (true, "If an account with this email exists, a password reset link has been sent.");
            }

            // Generate unique token
            var token = GeneratePasswordResetToken();
            var tokenHash = ComputePasswordResetTokenHash(token);

            // Store token in database
            var resetTokenEntity = new PasswordResetToken
            {
                UserId = user.Id,
                Token = token,
                TokenHash = tokenHash,
                ExpiresAt = DateTime.UtcNow.AddHours(1), // 1 hour expiry
                IsUsed = false,
                CreatedDate = DateTime.UtcNow
            };

            var resetTokenRepository = _unitOfWork.Repository<PasswordResetToken>();
            await resetTokenRepository.AddAsync(resetTokenEntity);
            await _unitOfWork.Save(CancellationToken.None);

            // Send email with reset link
            var fullResetLink = $"{resetLink}?token={token}&email={Uri.EscapeDataString(email)}";
            var emailSent = await _emailService.SendPasswordResetEmailAsync(email, user.UserName ?? "User", fullResetLink);

            if (!emailSent)
            {
                _logger.LogWarning($"Failed to send password reset email to {email}");
                return (true, "If an account with this email exists, a password reset link has been sent.");
            }

            _logger.LogInformation($"Password reset token sent to {email}");
            return (true, "If an account with this email exists, a password reset link has been sent.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing forgot password for email: {email}");
            return (false, "An error occurred while processing your request. Please try again.");
        }
    }

    public async Task<(bool Success, string Message)> ResetPasswordAsync(string token, string newPassword)
    {
        try
        {
            _logger.LogInformation("Processing password reset");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword))
            {
                return (false, "Invalid token or password");
            }

            // Find reset token
            var resetTokenRepository = _unitOfWork.Repository<PasswordResetToken>();
            var resetToken = await resetTokenRepository.Entities
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsUsed && rt.ExpiresAt > DateTime.UtcNow);

            if (resetToken == null)
            {
                _logger.LogWarning($"Invalid or expired reset token");
                return (false, "Invalid or expired reset token");
            }

            // Get user
            var user = await _userManager.FindByIdAsync(resetToken.UserId.ToString());
            if (user == null)
            {
                return (false, "User not found");
            }

            // Reset password
            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
            {
                var errors = string.Join(", ", removePasswordResult.Errors.Select(e => e.Description));
                _logger.LogError($"Failed to remove old password: {errors}");
                return (false, "Failed to reset password");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);
            if (!addPasswordResult.Succeeded)
            {
                var errors = string.Join(", ", addPasswordResult.Errors.Select(e => e.Description));
                _logger.LogError($"Failed to set new password: {errors}");
                return (false, addPasswordResult.Errors.FirstOrDefault()?.Description ?? "Failed to reset password");
            }

            // Mark token as used
            resetToken.IsUsed = true;
            resetToken.UsedAt = DateTime.UtcNow;
            await resetTokenRepository.UpdateAsync(resetToken);
            await _unitOfWork.Save(CancellationToken.None);

            _logger.LogInformation($"Password reset successfully for user: {user.Id}");
            return (true, "Password has been reset successfully. Please login with your new password.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password");
            return (false, "An error occurred while resetting your password. Please try again.");
        }
    }

    private async Task<string> GenerateJwtTokenAsync(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!),
            new("RoleId", user.RoleId.ToString()),
            new("jti", Guid.NewGuid().ToString()) // Unique token ID
        };

        // Add role claims if needed
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return _jwtService.GenerateToken(claims);
    }

    private async Task<string> GenerateAndStoreRefreshTokenAsync(User user, string? deviceInfo = null, string? ipAddress = null)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!)
        };

        var refreshToken = _jwtService.GenerateRefreshToken(claims);
        var tokenHash = _jwtService.ComputeTokenHash(refreshToken);

        // Store refresh token in database
        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // 7 days
            DeviceInfo = deviceInfo?.Length > 100 ? deviceInfo.Substring(0, 100) : deviceInfo,
            IpAddress = ipAddress
        };

        await _refreshTokenRepository.CreateAsync(refreshTokenEntity);

        return refreshToken;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private static string GeneratePasswordResetToken()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 32).ToUpper();
    }

    private static string ComputePasswordResetTokenHash(string token)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}

public class AuthResult
{
    public bool IsSuccess { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? Role { get; set; }
    public string? ErrorMessage { get; set; }

    public static AuthResult Success(string? accessToken, string? refreshToken, string userId, string userName, string email, string fullName, string role)
    {
        return new AuthResult
        {
            IsSuccess = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = userId,
            UserName = userName,
            Email = email,
            FullName = fullName,
            Role = role
        };
    }

    public static AuthResult Failed(string errorMessage)
    {
        return new AuthResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

