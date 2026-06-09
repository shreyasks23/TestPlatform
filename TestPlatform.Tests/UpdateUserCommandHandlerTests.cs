using FluentValidation;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Features.Users.UpdateUser;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Tests;

public sealed class UpdateUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCommandIsValid_UpdatesAndReturnsUser()
    {
        var userId = Guid.Parse("00000000-0000-0000-0000-930000000001");
        var existingUser = new User(userId, "ada@example.com", "Ada", "Lovelace");
        var repository = new UserRepositoryStub([existingUser]);
        var handler = new UpdateUserCommandHandler(repository, new UpdateUserCommandValidator());

        var result = await handler.Handle(
            new UpdateUserCommand(userId, "ada.augusta@example.com", "Ada", "Byron"),
            CancellationToken.None);

        Assert.Equal(userId, result.Id);
        Assert.Equal("ada.augusta@example.com", result.Email);
        Assert.Equal("Ada", result.FirstName);
        Assert.Equal("Byron", result.LastName);
        Assert.Equal("ada.augusta@example.com", existingUser.Email);
        Assert.Equal("Byron", existingUser.LastName);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ThrowsNotFound()
    {
        var handler = new UpdateUserCommandHandler(
            new UserRepositoryStub([]),
            new UpdateUserCommandValidator());

        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            handler.Handle(
                new UpdateUserCommand(Guid.CreateVersion7(), "ada@example.com", "Ada", "Lovelace"),
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenEmailBelongsToAnotherUser_ThrowsInvalidOperationException()
    {
        var firstUser = new User(
            Guid.Parse("00000000-0000-0000-0000-930000000002"),
            "ada@example.com",
            "Ada",
            "Lovelace");
        var secondUser = new User(
            Guid.Parse("00000000-0000-0000-0000-930000000003"),
            "grace@example.com",
            "Grace",
            "Hopper");

        var handler = new UpdateUserCommandHandler(
            new UserRepositoryStub([firstUser, secondUser]),
            new UpdateUserCommandValidator());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(
                new UpdateUserCommand(firstUser.Id, "grace@example.com", "Ada", "Lovelace"),
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenCommandIsInvalid_ThrowsValidationException()
    {
        var existingUser = new User("ada@example.com", "Ada", "Lovelace");
        var handler = new UpdateUserCommandHandler(
            new UserRepositoryStub([existingUser]),
            new UpdateUserCommandValidator());

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(
                new UpdateUserCommand(existingUser.Id, string.Empty, "Ada", "Lovelace"),
                CancellationToken.None));
    }
}
