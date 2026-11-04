using TestPlatform.Domain.Entities;

namespace TestPlatform.Application.Common.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);
    Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
}
