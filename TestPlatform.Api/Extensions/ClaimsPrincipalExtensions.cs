using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Globalization;
using TestPlatform.Application.Common.Security;

namespace TestPlatform.Api.Extensions;

internal static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var subject = user.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(subject, out var userId)
            ? userId
            : throw new InvalidOperationException("The authenticated token does not include a valid user identifier.");
    }

    public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRole(AuthConstants.Roles.Admin);

    public static Guid GetSessionId(this ClaimsPrincipal user)
    {
        var sessionId = user.FindFirstValue(AuthConstants.Claims.SessionId);
        return Guid.TryParse(sessionId, out var parsedSessionId)
            ? parsedSessionId
            : throw new InvalidOperationException("The authenticated token does not include a valid session identifier.");
    }

    public static int GetTokenVersion(this ClaimsPrincipal user)
    {
        var tokenVersion = user.FindFirstValue(AuthConstants.Claims.TokenVersion);
        return int.TryParse(tokenVersion, NumberStyles.None, CultureInfo.InvariantCulture, out var parsedTokenVersion) && parsedTokenVersion > 0
            ? parsedTokenVersion
            : throw new InvalidOperationException("The authenticated token does not include a valid token version.");
    }
}
