using FluentValidation;
using TestPlatform.Application.Common.Security;
using TestPlatform.Application.Features.Users.CreateUser;

namespace TestPlatform.Tests;

public sealed class CreateUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCommandIsValid_CreatesUserWithHashedPasswordAndAdminRole()
    {
        var repository = new UserRepositoryStub([]);
        var handler = new CreateUserCommandHandler(
            repository,
            new FakePasswordHasher(),
            new CreateUserCommandValidator());

        var result = await handler.Handle(
            new CreateUserCommand("ada@example.com", "Ada", "Lovelace", "P@ssword123"),
            CancellationToken.None);

        Assert.Equal("ada@example.com", result.Email);
        Assert.NotNull(repository.AddedUser);
        Assert.Equal(AuthConstants.Roles.Admin, repository.AddedUser!.Role);
        Assert.Equal("hashed::P@ssword123", repository.AddedUser.PasswordHash);
    }

    [Fact]
    public async Task Handle_WhenPasswordIsTooShort_ThrowsValidationException()
    {
        var repository = new UserRepositoryStub([]);
        var handler = new CreateUserCommandHandler(
            repository,
            new FakePasswordHasher(),
            new CreateUserCommandValidator());

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new CreateUserCommand("ada@example.com", "Ada", "Lovelace", "short"), CancellationToken.None));
    }
}
