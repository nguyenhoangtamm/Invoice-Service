using Invoice.Domain.Entities;
using Invoice.Domain.Interfaces.Repositories;
using Invoice.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Invoice.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
    {
        return await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);
    }

    public async Task<RefreshToken?> GetByTokenHashAndUserIdAsync(string tokenHash, int userId)
    {
        return await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash && x.UserId == userId);
    }

    public async Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(int userId)
    {
        return await _context.RefreshTokens
            .Where(x => x.UserId == userId && x.IsActive)
            .ToListAsync();
    }

    public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Remove(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteExpiredTokensAsync()
    {
        var expiredTokens = await _context.RefreshTokens
            .Where(x => x.IsExpired)
            .ToListAsync();

        _context.RefreshTokens.RemoveRange(expiredTokens);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllUserTokensAsync(int userId)
    {
        var userTokens = await _context.RefreshTokens
            .Where(x => x.UserId == userId && !x.IsRevoked)
            .ToListAsync();

        foreach (var token in userTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }
}

