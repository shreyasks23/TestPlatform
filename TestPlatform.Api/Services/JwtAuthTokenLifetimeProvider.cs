using Microsoft.Extensions.Options;
using TestPlatform.Api.Configuration;
using TestPlatform.Application.Common.Interfaces.Security;

namespace TestPlatform.Api.Services;

internal sealed class JwtAuthTokenLifetimeProvider(IOptions<JwtOptions> options) : IAuthTokenLifetimeProvider
{
    private readonly JwtOptions _options = options.Value;

    public TimeSpan RefreshTokenLifetime => TimeSpan.FromDays(_options.RefreshTokenLifetimeDays);
}
