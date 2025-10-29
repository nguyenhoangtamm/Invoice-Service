using Invoice.Domain.Entities;

namespace Invoice.Domain.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
    Task<RefreshToken?> GetByTokenHashAndUserIdAsync(string tokenHash, int userId);
    Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(int userId);
    Task<RefreshToken> CreateAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
    Task DeleteAsync(RefreshToken refreshToken);
    Task DeleteExpiredTokensAsync();
    Task RevokeAllUserTokensAsync(int userId);
}

