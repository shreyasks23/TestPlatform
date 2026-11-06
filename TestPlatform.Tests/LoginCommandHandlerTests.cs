using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Security;
using TestPlatform.Application.Features.Auth.Login;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Tests;

public sealed class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCredentialsAreValid_IssuesAccessAndRefreshTokens()
    {
        var user = BuildUser(Guid.Parse("00000000-0000-0000-0000-930000000001"), "shreyasks@mail.com", "hashed::P@SSw0rd");
        var handler = new LoginCommandHandler(
            new UserRepositoryStub([user]),
            new FakePasswordHasher(),
            new FakeAccessTokenService(DateTimeOffset.Parse("2026-06-09T10:15:00Z")),
            new FakeRefreshTokenService(),
            new RefreshTokenRepositoryStub([]),
            new FakeAuthTokenLifetimeProvider(TimeSpan.FromDays(7)),
            new FakeDateTimeProvider(DateTimeOffset.Parse("2026-06-09T10:00:00Z")),
            new LoginCommandValidator());

        var result = await handler.Handle(new LoginCommand("shreyasks@mail.com", "P@SSw0rd"), CancellationToken.None);

        Assert.StartsWith("access::shreyasks@mail.com::", result.AccessToken, StringComparison.Ordinal);
        Assert.Equal("refresh-token-1", result.RefreshToken);
        Assert.Equal(DateTimeOffset.Parse("2026-06-16T10:00:00Z"), result.RefreshTokenExpiresAtUtc);
    }

    [Fact]
    public async Task Handle_WhenPasswordIsInvalid_ThrowsAuthenticationFailedException()
    {
        var user = BuildUser(Guid.Parse("00000000-0000-0000-0000-930000000002"), "shreyasks@mail.com", "hashed::P@SSw0rd");
        var handler = new LoginCommandHandler(
            new UserRepositoryStub([user]),
            new FakePasswordHasher(),
            new FakeAccessTokenService(DateTimeOffset.Parse("2026-06-09T10:15:00Z")),
            new FakeRefreshTokenService(),
            new RefreshTokenRepositoryStub([]),
            new FakeAuthTokenLifetimeProvider(TimeSpan.FromDays(7)),
            new FakeDateTimeProvider(DateTimeOffset.Parse("2026-06-09T10:00:00Z")),
            new LoginCommandValidator());

        await Assert.ThrowsAsync<AuthenticationFailedException>(() =>
            handler.Handle(new LoginCommand("shreyasks@mail.com", "wrong-password"), CancellationToken.None));
    }

    private static User BuildUser(Guid id, string email, string passwordHash)
        => new(id, email, "Shreyas", "K S", passwordHash, AuthConstants.Roles.Admin);
}
