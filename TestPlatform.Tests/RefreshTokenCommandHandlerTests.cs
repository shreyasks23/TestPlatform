using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Security;
using TestPlatform.Application.Features.Auth.Refresh;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Tests;

public sealed class RefreshTokenCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenRefreshTokenIsValid_RotatesAndReturnsNewTokenPair()
    {
        var user = new User(
            Guid.Parse("00000000-0000-0000-0000-930000000003"),
            "shreyasks@mail.com",
            "Shreyas",
            "K S",
            "hashed::P@SSw0rd",
            AuthConstants.Roles.Admin);
        var existingRefreshToken = new RefreshToken(
            user.Id,
            Guid.Parse("00000000-0000-0000-0000-930000000099"),
            "hash::existing-refresh-token",
            DateTimeOffset.Parse("2026-06-09T09:00:00Z"),
            DateTimeOffset.Parse("2026-06-16T09:00:00Z"));
        var handler = new RefreshTokenCommandHandler(
            new RefreshTokenRepositoryStub([existingRefreshToken]),
            new UserRepositoryStub([user]),
            new FakeRefreshTokenService(),
            new FakeAccessTokenService(DateTimeOffset.Parse("2026-06-09T10:15:00Z")),
            new FakeAuthTokenLifetimeProvider(TimeSpan.FromDays(7)),
            new FakeDateTimeProvider(DateTimeOffset.Parse("2026-06-09T10:00:00Z")),
            new RefreshTokenCommandValidator());

        var result = await handler.Handle(new RefreshTokenCommand("existing-refresh-token"), CancellationToken.None);

        Assert.Equal($"access::shreyasks@mail.com::{existingRefreshToken.SessionId}", result.AccessToken);
        Assert.Equal("refresh-token-1", result.RefreshToken);
        Assert.NotNull(existingRefreshToken.RevokedAtUtc);
    }

    [Fact]
    public async Task Handle_WhenRefreshTokenIsInvalid_ThrowsAuthenticationFailedException()
    {
        var handler = new RefreshTokenCommandHandler(
            new RefreshTokenRepositoryStub([]),
            new UserRepositoryStub([]),
            new FakeRefreshTokenService(),
            new FakeAccessTokenService(DateTimeOffset.Parse("2026-06-09T10:15:00Z")),
            new FakeAuthTokenLifetimeProvider(TimeSpan.FromDays(7)),
            new FakeDateTimeProvider(DateTimeOffset.Parse("2026-06-09T10:00:00Z")),
            new RefreshTokenCommandValidator());

        await Assert.ThrowsAsync<AuthenticationFailedException>(() =>
            handler.Handle(new RefreshTokenCommand("missing-token"), CancellationToken.None));
    }
}
