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

    public Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        => Task.CompletedTask;
}
