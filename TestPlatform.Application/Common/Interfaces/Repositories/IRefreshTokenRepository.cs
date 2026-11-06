using TestPlatform.Domain.Entities;

namespace TestPlatform.Application.Common.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);
    Task<bool> HasActiveSessionAsync(Guid userId, Guid sessionId, DateTimeOffset now, CancellationToken cancellationToken);
    Task RevokeSessionAsync(Guid userId, Guid sessionId, DateTimeOffset revokedAtUtc, CancellationToken cancellationToken);
    Task RevokeAllForUserAsync(Guid userId, DateTimeOffset revokedAtUtc, CancellationToken cancellationToken);
    Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
}
