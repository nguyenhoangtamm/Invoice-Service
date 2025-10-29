using System.Security.Claims;

namespace Invoice.Domain.Common.Security;

public interface IJwtService
{
    string GenerateToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken(IEnumerable<Claim> claims);
    ClaimsPrincipal? ValidateToken(string token);
    
    // New security methods
    string ComputeTokenHash(string token);
    Task<bool> IsTokenBlacklistedAsync(string tokenHash);
    Task BlacklistTokenAsync(string token, string tokenType, int? userId = null, string? reason = null);
    Task<bool> IsRefreshTokenValidAsync(string tokenHash, int userId);
    Task RevokeRefreshTokenAsync(string tokenHash, int userId);
    Task RevokeAllUserRefreshTokensAsync(int userId);
    Task CleanupExpiredTokensAsync();
}



