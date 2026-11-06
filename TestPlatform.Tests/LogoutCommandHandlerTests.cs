using TestPlatform.Application.Common.Security;
using TestPlatform.Application.Features.Auth.Logout;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Tests;

public sealed class LogoutCommandHandlerTests
{
    [Fact]
    public async Task Handle_RevokesOnlyTheAuthenticatedSession()
    {
        var now = DateTimeOffset.Parse("2026-11-06T10:00:00Z");
        var userId = Guid.CreateVersion7();
        var currentSessionId = Guid.CreateVersion7();
        var otherSessionId = Guid.CreateVersion7();
        var currentToken = new RefreshToken(userId, currentSessionId, "hash::current", now.AddMinutes(-1), now.AddDays(7));
        var otherToken = new RefreshToken(userId, otherSessionId, "hash::other", now.AddMinutes(-1), now.AddDays(7));
        var repository = new RefreshTokenRepositoryStub([currentToken, otherToken]);
        var handler = new LogoutCommandHandler(repository, new FakeDateTimeProvider(now), new LogoutCommandValidator());

        await handler.Handle(new LogoutCommand(userId, currentSessionId), CancellationToken.None);

        Assert.NotNull(currentToken.RevokedAtUtc);
        Assert.Null(otherToken.RevokedAtUtc);
        Assert.False(await repository.HasActiveSessionAsync(userId, currentSessionId, now, CancellationToken.None));
        Assert.True(await repository.HasActiveSessionAsync(userId, otherSessionId, now, CancellationToken.None));
    }
}
