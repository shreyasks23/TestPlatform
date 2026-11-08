using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Features.Users.GetUserById;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Tests;

public sealed class GetUserByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenUserExists_ReturnsMappedUser()
    {
        var user = new User(
            Guid.Parse("00000000-0000-0000-0000-930000000004"),
            "ada@example.com",
            "Ada",
            "Lovelace", "hashed::pw", "TestAuthor");
        var handler = new GetUserByIdQueryHandler(new UserRepositoryStub([user]));

        var result = await handler.Handle(new GetUserByIdQuery(user.Id), CancellationToken.None);

        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.FirstName, result.FirstName);
        Assert.Equal(user.LastName, result.LastName);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ThrowsNotFound()
    {
        var handler = new GetUserByIdQueryHandler(new UserRepositoryStub([]));

        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            handler.Handle(new GetUserByIdQuery(Guid.CreateVersion7()), CancellationToken.None));
    }
}
