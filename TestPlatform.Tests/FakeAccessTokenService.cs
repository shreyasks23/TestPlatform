using TestPlatform.Application.Common.Interfaces.Security;
using TestPlatform.Application.Common.Models;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Tests;

internal sealed class FakeAccessTokenService(DateTimeOffset expiresAtUtc) : IAccessTokenService
{
    public AccessTokenResult CreateToken(User user, Guid sessionId)
        => new($"access::{user.Email}::{sessionId}", expiresAtUtc);
}
