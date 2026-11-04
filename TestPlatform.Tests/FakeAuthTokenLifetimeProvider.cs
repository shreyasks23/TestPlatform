using TestPlatform.Application.Common.Interfaces.Security;

namespace TestPlatform.Tests;

internal sealed class FakeAuthTokenLifetimeProvider(TimeSpan refreshTokenLifetime) : IAuthTokenLifetimeProvider
{
    public TimeSpan RefreshTokenLifetime { get; } = refreshTokenLifetime;
}
