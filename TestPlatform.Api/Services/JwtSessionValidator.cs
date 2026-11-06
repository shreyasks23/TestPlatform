using System.Security.Claims;
using TestPlatform.Api.Extensions;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Application.Common.Interfaces.Security;

namespace TestPlatform.Api.Services;

internal sealed class JwtSessionValidator(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IDateTimeProvider dateTimeProvider)
{
    public async Task<bool> IsValidAsync(ClaimsPrincipal principal, CancellationToken cancellationToken)
    {
        try
        {
            var userId = principal.GetUserId();
            var sessionId = principal.GetSessionId();
            var tokenVersion = principal.GetTokenVersion();
            var user = await userRepository.GetByIdAsync(userId, cancellationToken);

            return user is not null &&
                   user.TokenVersion == tokenVersion &&
                   await refreshTokenRepository.HasActiveSessionAsync(userId, sessionId, dateTimeProvider.UtcNow, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}
