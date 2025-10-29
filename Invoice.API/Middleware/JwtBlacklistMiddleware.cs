using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Invoice.Domain.Common.Security;
using Invoice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Invoice.API.Middleware;

public class JwtBlacklistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtSettings _settings;

    public JwtBlacklistMiddleware(RequestDelegate next, IOptions<JwtSettings> options)
    {
        _next = next;
        _settings = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? token = null;

        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = authHeader.Substring("Bearer ".Length).Trim();
        }

        if (string.IsNullOrEmpty(token) && context.Request.Query.TryGetValue("access_token", out var accessTokenValues))
        {
            token = accessTokenValues.FirstOrDefault();
        }

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                // First validate the token format and signature
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

                // Check if token is blacklisted using scoped service
                using (var scope = context.RequestServices.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var tokenHash = ComputeTokenHash(token);
                    var currentTime = DateTime.UtcNow;

                    var isBlacklisted = await dbContext.TokenBlacklists
                        .AnyAsync(x => x.TokenHash == tokenHash && x.ExpiresAt > currentTime);

                    if (isBlacklisted)
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Token has been revoked");
                        return;
                    }
                }

                context.User = principal;
            }
            catch
            {
                // Token validation failed, continue without user context
            }
        }

        await _next(context);
    }

    private static string ComputeTokenHash(string token)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}

