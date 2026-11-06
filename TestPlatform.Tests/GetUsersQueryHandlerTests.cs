using TestPlatform.Application.Features.Users.GetUsers;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Tests;

public sealed class GetUsersQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenUsersExist_ReturnsAllMappedUsers()
    {
        var firstUser = new User(
            Guid.Parse("00000000-0000-0000-0000-930000000005"),
            "ada@example.com",
            "Ada",
            "Lovelace", "hashed::pw", "TestAuthor");
        var secondUser = new User(
            Guid.Parse("00000000-0000-0000-0000-930000000006"),
            "grace@example.com",
            "Grace",
            "Hopper", "hashed::pw", "TestTaker");

        var handler = new GetUsersQueryHandler(new UserRepositoryStub([firstUser, secondUser]));

        var result = await handler.Handle(new GetUsersQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.Collection(
            result,
            user =>
            {
                Assert.Equal(firstUser.Id, user.Id);
                Assert.Equal(firstUser.Email, user.Email);
            },
            user =>
            {
                Assert.Equal(secondUser.Id, user.Id);
                Assert.Equal(secondUser.Email, user.Email);
            });
    }

    [Fact]
    public async Task Handle_WhenNoUsersExist_ReturnsEmptyCollection()
    {
        var handler = new GetUsersQueryHandler(new UserRepositoryStub([]));

        var result = await handler.Handle(new GetUsersQuery(), CancellationToken.None);

        Assert.Empty(result);
    }
}
