using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Tests;

internal sealed class RefreshTokenRepositoryStub(params IEnumerable<RefreshToken> seededRefreshTokens) : IRefreshTokenRepository
{
    private readonly List<RefreshToken> _refreshTokens = [.. seededRefreshTokens];

    public Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        _refreshTokens.Add(refreshToken);
        return Task.CompletedTask;
    }

    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken)
        => Task.FromResult(_refreshTokens.SingleOrDefault(refreshToken => refreshToken.TokenHash == tokenHash));

    public Task<bool> HasActiveSessionAsync(Guid userId, Guid sessionId, DateTimeOffset now, CancellationToken cancellationToken)
        => Task.FromResult(_refreshTokens.Any(refreshToken =>
            refreshToken.UserId == userId &&
            refreshToken.SessionId == sessionId &&
            refreshToken.IsActive(now)));

    public Task RevokeSessionAsync(Guid userId, Guid sessionId, DateTimeOffset revokedAtUtc, CancellationToken cancellationToken)
    {
        foreach (var refreshToken in _refreshTokens.Where(token => token.UserId == userId && token.SessionId == sessionId))
            refreshToken.Revoke(revokedAtUtc);

        return Task.CompletedTask;
    }

    public Task RevokeAllForUserAsync(Guid userId, DateTimeOffset revokedAtUtc, CancellationToken cancellationToken)
    {
        foreach (var refreshToken in _refreshTokens.Where(token => token.UserId == userId))
            refreshToken.Revoke(revokedAtUtc);

        return Task.CompletedTask;
    }

    public Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        => Task.CompletedTask;
}
