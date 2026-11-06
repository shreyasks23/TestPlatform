using FluentValidation;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Security;
using TestPlatform.Application.Features.Users.UpdateUser;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Tests;

public sealed class UpdateUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCommandIsValid_UpdatesProfileAndRole()
    {
        var userId = Guid.CreateVersion7();
        var user = new User(userId, "ada@example.com", "Ada", "Lovelace", "hashed::pw", AuthConstants.Roles.TestTaker);
        var handler = new UpdateUserCommandHandler(new UserRepositoryStub([user]), new UpdateUserCommandValidator());

        var result = await handler.Handle(new UpdateUserCommand(userId, "ada.augusta@example.com", "Ada", "Byron", AuthConstants.Roles.TestAuthor), CancellationToken.None);

        Assert.Equal(AuthConstants.Roles.TestAuthor, result.Role);
        Assert.Equal("ada.augusta@example.com", user.Email);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ThrowsNotFound()
    {
        var handler = new UpdateUserCommandHandler(new UserRepositoryStub([]), new UpdateUserCommandValidator());

        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            handler.Handle(new UpdateUserCommand(Guid.CreateVersion7(), "ada@example.com", "Ada", "Lovelace", AuthConstants.Roles.TestTaker), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenRoleIsInvalid_ThrowsValidationException()
    {
        var user = new User("ada@example.com", "Ada", "Lovelace", "hashed::pw", AuthConstants.Roles.TestTaker);
        var handler = new UpdateUserCommandHandler(new UserRepositoryStub([user]), new UpdateUserCommandValidator());

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new UpdateUserCommand(user.Id, user.Email, "Ada", "Lovelace", "Unknown"), CancellationToken.None));
    }
}
