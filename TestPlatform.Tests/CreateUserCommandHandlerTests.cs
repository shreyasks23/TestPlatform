using FluentValidation;
using TestPlatform.Application.Common.Security;
using TestPlatform.Application.Features.Users.CreateUser;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Tests;

public sealed class CreateUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCommandIsValid_CreatesUserWithRequestedRole()
    {
        var repository = new UserRepositoryStub([]);
        var handler = new CreateUserCommandHandler(repository, new FakePasswordHasher(), new CreateUserCommandValidator());

        var result = await handler.Handle(new CreateUserCommand("ada@example.com", "Ada", "Lovelace", "P@ssword1", AuthConstants.Roles.TestAuthor), CancellationToken.None);

        Assert.Equal(AuthConstants.Roles.TestAuthor, result.Role);
        Assert.Equal("hashed::P@ssword1", repository.AddedUser!.PasswordHash);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ThrowsInvalidOperationException()
    {
        var existingUser = new User("ada@example.com", "Ada", "Lovelace", "hashed::pw", AuthConstants.Roles.TestTaker);
        var handler = new CreateUserCommandHandler(new UserRepositoryStub([existingUser]), new FakePasswordHasher(), new CreateUserCommandValidator());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new CreateUserCommand("ada@example.com", "Another", "User", "P@ssword1", AuthConstants.Roles.TestTaker), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenRoleIsInvalid_ThrowsValidationException()
    {
        var handler = new CreateUserCommandHandler(new UserRepositoryStub([]), new FakePasswordHasher(), new CreateUserCommandValidator());

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new CreateUserCommand("ada@example.com", "Ada", "Lovelace", "P@ssword1", "Unknown"), CancellationToken.None));
    }
}
