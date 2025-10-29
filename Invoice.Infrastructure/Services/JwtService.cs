using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Invoice.Domain.Common.Security;
using Invoice.Domain.Entities;
using Invoice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Invoice.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettings _settings;
    private readonly ApplicationDbContext _context;

    public JwtService(IOptions<JwtSettings> options, ApplicationDbContext context)
    {
        _settings = options.Value;
        _context = context;
    }

    public string GenerateToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_settings.ExpiresInMinutes);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        // Refresh token có th?i h?n dài h?n (7 ngày)
        var expires = DateTime.UtcNow.AddDays(7);

        // Thêm claim ??c bi?t ?? phân bi?t refresh token
        var refreshClaims = new List<Claim>(claims)
        {
            new Claim("token_type", "refresh"),
            new Claim("jti", Guid.NewGuid().ToString()) // Unique token ID
        };

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: refreshClaims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_settings.Key);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _settings.Issuer,
                ValidateAudience = true,
                ValidAudience = _settings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    public string ComputeTokenHash(string token)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    public async Task<bool> IsTokenBlacklistedAsync(string tokenHash)
    {
        var currentTime = DateTime.UtcNow;
        return await _context.TokenBlacklists
            .AnyAsync(x => x.TokenHash == tokenHash && x.ExpiresAt > currentTime);
    }

    public async Task BlacklistTokenAsync(string token, string tokenType, int? userId = null, string? reason = null)
    {
        var tokenHash = ComputeTokenHash(token);
        
        // Get token expiration from JWT
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(token);
        var expires = jsonToken.ValidTo;

        var blacklistEntry = new TokenBlacklist
        {
            TokenHash = tokenHash,
            TokenType = tokenType,
            ExpiresAt = expires,
            UserId = userId,
            Reason = reason ?? "User logout"
        };

        _context.TokenBlacklists.Add(blacklistEntry);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsRefreshTokenValidAsync(string tokenHash, int userId)
    {
        var currentTime = DateTime.UtcNow;
        return await _context.RefreshTokens
            .AnyAsync(x => x.TokenHash == tokenHash && 
                          x.UserId == userId && 
                          !x.IsRevoked &&
                          x.ExpiresAt > currentTime);
    }

    public async Task RevokeRefreshTokenAsync(string tokenHash, int userId)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash && x.UserId == userId);

        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task RevokeAllUserRefreshTokensAsync(int userId)
    {
        var refreshTokens = await _context.RefreshTokens
            .Where(x => x.UserId == userId && !x.IsRevoked)
            .ToListAsync();

        foreach (var token in refreshTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task CleanupExpiredTokensAsync()
    {
        var currentTime = DateTime.UtcNow;
        
        // Use raw DateTime comparison instead of computed properties
        var expiredRefreshTokens = await _context.RefreshTokens
            .Where(x => x.ExpiresAt <= currentTime)
            .ToListAsync();

        var expiredBlacklistTokens = await _context.TokenBlacklists
            .Where(x => x.ExpiresAt <= currentTime)
            .ToListAsync();

        _context.RefreshTokens.RemoveRange(expiredRefreshTokens);
        _context.TokenBlacklists.RemoveRange(expiredBlacklistTokens);

        await _context.SaveChangesAsync();
    }
}


