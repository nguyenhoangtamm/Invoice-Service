using System.Security.Claims;
using Invoice.Domain.Common.Security;
using Invoice.Domain.Entities;
using Invoice.Domain.Enums;
using Invoice.Domain.Interfaces;
using Invoice.Domain.Interfaces.Repositories;
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
}

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        RoleManager<Role> roleManager,
        IJwtService jwtService,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AuthResult> LoginAsync(string usernameOrEmail, string password, string? deviceInfo = null, string? ipAddress = null)
    {
        try
        {
            User? user = null;

            // T�m user b?ng email tr??c
            if (IsValidEmail(usernameOrEmail))
            {
                user = await _userManager.Users
                    .Include(u => u.Profile)
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == usernameOrEmail);
            }

            // N?u kh�ng t�m th?y b?ng email ho?c input kh�ng ph?i email, th� t�m b?ng username
            if (user == null)
            {
                user = await _userManager.Users
                    .Include(u => u.Profile)
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

            // Get full name from profile if exists, otherwise use username
            var fullName = user.Profile?.Fullname ?? user.UserName ?? "Unknown";

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

            // Ki?m tra username ?� t?n t?i ch?a
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
                EmailConfirmed = true // Set to true for now, implement email confirmation later if needed
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return AuthResult.Failed($"User creation failed: {errors}");
            }

            // Create Profile for the new user
            try
            {
                var profile = new Profile
                {
                    UserId = user.Id,
                    Fullname = fullname,
                    Email = email,
                    Gender = gender,
                    BirthDate = birthDate,
                    Address = address,
                    Bio = bio,
                    PhoneNumber = phoneNumber,
                    AvatarUrl = avatarUrl
                };

                var profileRepository = _unitOfWork.Repository<Profile>();
                await profileRepository.AddAsync(profile);
                await _unitOfWork.Save(CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating profile for new user: {UserId}", user.Id);
                // Continue without failing - profile creation is secondary
            }

            // Load user with role information
            user = await _userManager.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            // Get role information
            var roleName = user?.Role?.Name ?? "Unknown";

            var accessToken = await GenerateJwtTokenAsync(user!);
            var refreshToken = await GenerateAndStoreRefreshTokenAsync(user!);

            return AuthResult.Success(
                accessToken,
                refreshToken,
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

            // Ki?m tra xem c� ph?i refresh token kh�ng
            var tokenType = principal.FindFirst("token_type")?.Value;
            if (tokenType != "refresh")
            {
                return AuthResult.Failed("Invalid token type.");
            }

            // L?y user ID t? token
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return AuthResult.Failed("Invalid user ID in token.");
            }

            // Ki?m tra refresh token trong database
            var tokenHash = _jwtService.ComputeTokenHash(refreshToken);

            // Use JWT service to validate refresh token
            var isValidToken = await _jwtService.IsRefreshTokenValidAsync(tokenHash, userId);
            if (!isValidToken)
            {
                return AuthResult.Failed("Refresh token not found or expired.");
            }

            // Get stored token for device info
            var storedToken = await _refreshTokenRepository.GetByTokenHashAndUserIdAsync(tokenHash, userId);

            // T�m user
            var user = await _userManager.Users
                .Include(u => u.Profile)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return AuthResult.Failed("User not found.");
            }

            // Revoke old refresh token (token rotation)
            await _jwtService.RevokeRefreshTokenAsync(tokenHash, userId);

            // T?o token m?i
            var newAccessToken = await GenerateJwtTokenAsync(user);
            var newRefreshToken = await GenerateAndStoreRefreshTokenAsync(user, storedToken?.DeviceInfo, storedToken?.IpAddress);

            // Get user info
            var roleName = user.Role?.Name ?? "Unknown";
            var fullName = user.Profile?.Fullname ?? user.UserName ?? "Unknown";

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
                    .Include(u => u.Profile)
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
            DeviceInfo = deviceInfo,
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

    public static AuthResult Success(string accessToken, string refreshToken, string userId, string userName, string email, string fullName, string role)
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

