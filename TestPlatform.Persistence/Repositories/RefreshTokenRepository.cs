using Microsoft.EntityFrameworkCore;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Persistence.Repositories;

internal sealed class RefreshTokenRepository(TestPlatformDbContext dbContext) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
        => dbContext.RefreshTokens.SingleOrDefaultAsync(refreshToken => refreshToken.TokenHash == tokenHash, cancellationToken);

    public Task<bool> HasActiveSessionAsync(Guid userId, Guid sessionId, DateTimeOffset now, CancellationToken cancellationToken)
        => dbContext.RefreshTokens.AnyAsync(
            refreshToken => refreshToken.UserId == userId &&
                            refreshToken.SessionId == sessionId &&
                            refreshToken.RevokedAtUtc == null &&
                            refreshToken.ExpiresAtUtc > now,
            cancellationToken);

    public Task RevokeSessionAsync(Guid userId, Guid sessionId, DateTimeOffset revokedAtUtc, CancellationToken cancellationToken)
        => RevokeAsync(
            dbContext.RefreshTokens.Where(refreshToken => refreshToken.UserId == userId && refreshToken.SessionId == sessionId),
            revokedAtUtc,
            cancellationToken);

    public Task RevokeAllForUserAsync(Guid userId, DateTimeOffset revokedAtUtc, CancellationToken cancellationToken)
        => RevokeAsync(
            dbContext.RefreshTokens.Where(refreshToken => refreshToken.UserId == userId),
            revokedAtUtc,
            cancellationToken);

    public Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        => dbContext.SaveChangesAsync(cancellationToken);

    private async Task RevokeAsync(
        IQueryable<RefreshToken> query,
        DateTimeOffset revokedAtUtc,
        CancellationToken cancellationToken)
    {
        var refreshTokens = await query
            .Where(refreshToken => refreshToken.RevokedAtUtc == null)
            .ToArrayAsync(cancellationToken);

        foreach (var refreshToken in refreshTokens)
            refreshToken.Revoke(revokedAtUtc);

        if (refreshTokens.Length > 0)
            await dbContext.SaveChangesAsync(cancellationToken);
    }
}
