using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Globalization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TestPlatform.Api.Configuration;
using TestPlatform.Application.Common.Interfaces.Security;
using TestPlatform.Application.Common.Models;
using TestPlatform.Application.Common.Security;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Api.Services;

internal sealed class JwtAccessTokenService(
    IOptions<JwtOptions> options,
    IDateTimeProvider dateTimeProvider) : IAccessTokenService
{
    private readonly JwtOptions _options = options.Value;

    public AccessTokenResult CreateToken(User user, Guid sessionId)
    {
        if (sessionId == Guid.Empty)
            throw new ArgumentException("Session ID cannot be empty", nameof(sessionId));

        var issuedAtUtc = dateTimeProvider.UtcNow;
        var expiresAtUtc = issuedAtUtc.AddMinutes(_options.AccessTokenLifetimeMinutes);
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(AuthConstants.Claims.SessionId, sessionId.ToString()),
            new Claim(AuthConstants.Claims.TokenVersion, user.TokenVersion.ToString(CultureInfo.InvariantCulture))
        };
        claims.AddRange(AuthConstants.GetPermissions(user.Role).Select(permission => new Claim("permission", permission)));

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: issuedAtUtc.UtcDateTime,
            expires: expiresAtUtc.UtcDateTime,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)),
                SecurityAlgorithms.HmacSha256));

        return new AccessTokenResult(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAtUtc);
    }
}
