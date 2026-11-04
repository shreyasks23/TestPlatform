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

    public Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        => dbContext.SaveChangesAsync(cancellationToken);
}
