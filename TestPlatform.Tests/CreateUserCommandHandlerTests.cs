using FluentValidation;
using TestPlatform.Application.Common.Security;
using TestPlatform.Application.Features.Users.CreateUser;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Tests;

public sealed class CreateUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCommandIsValid_CreatesAndReturnsUser()
    {
        var repository = new UserRepositoryStub([]);
        var handler = new CreateUserCommandHandler(repository, new CreateUserCommandValidator());

        var command = new CreateUserCommand("ada@example.com", "Ada", "Lovelace");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("ada@example.com", result.Email);
        Assert.Equal("Ada", result.FirstName);
        Assert.Equal("Lovelace", result.LastName);
        Assert.NotNull(repository.AddedUser);
        Assert.Equal(result.Id, repository.AddedUser!.Id);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ThrowsInvalidOperationException()
    {
        var existingUser = new User("ada@example.com", "Ada", "Lovelace");
        var repository = new UserRepositoryStub([existingUser]);
        var handler = new CreateUserCommandHandler(repository, new CreateUserCommandValidator());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(
                new CreateUserCommand("ada@example.com", "Another", "User"),
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenEmailIsInvalid_ThrowsValidationException()
    {
        var repository = new UserRepositoryStub([]);
        var handler = new CreateUserCommandHandler(repository, new CreateUserCommandValidator());

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(
                new CreateUserCommand("not-an-email", "Ada", "Lovelace"),
                CancellationToken.None));
    }
}
