using TestPlatform.Application.Common.Security;
using TestPlatform.Application.Features.Auth.LogoutAll;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Tests;

public sealed class LogoutAllCommandHandlerTests
{
    [Fact]
    public async Task Handle_RevokesEverySessionAndIncrementsTokenVersion()
    {
        var now = DateTimeOffset.Parse("2026-11-06T10:00:00Z");
        var user = new User(Guid.CreateVersion7(), "ada@example.com", "Ada", "Lovelace", "hashed::pw", AuthConstants.Roles.TestAuthor);
        var otherUser = new User(Guid.CreateVersion7(), "grace@example.com", "Grace", "Hopper", "hashed::pw", AuthConstants.Roles.TestTaker);
        var firstToken = new RefreshToken(user.Id, Guid.CreateVersion7(), "hash::first", now.AddMinutes(-1), now.AddDays(7));
        var secondToken = new RefreshToken(user.Id, Guid.CreateVersion7(), "hash::second", now.AddMinutes(-1), now.AddDays(7));
        var otherUserToken = new RefreshToken(otherUser.Id, Guid.CreateVersion7(), "hash::other", now.AddMinutes(-1), now.AddDays(7));
        var handler = new LogoutAllCommandHandler(
            new UserRepositoryStub([user, otherUser]),
            new RefreshTokenRepositoryStub([firstToken, secondToken, otherUserToken]),
            new FakeDateTimeProvider(now),
            new LogoutAllCommandValidator());

        await handler.Handle(new LogoutAllCommand(user.Id), CancellationToken.None);

        Assert.Equal(2, user.TokenVersion);
        Assert.NotNull(firstToken.RevokedAtUtc);
        Assert.NotNull(secondToken.RevokedAtUtc);
        Assert.Null(otherUserToken.RevokedAtUtc);
    }
}
