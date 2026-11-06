using FluentValidation;
using MediatR;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Application.Common.Interfaces.Security;
using TestPlatform.Application.Common.Security;
using TestPlatform.Application.Features.Users;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Application.Features.Users.CreateUser;

public sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IValidator<CreateUserCommand> validator) : IRequestHandler<CreateUserCommand, UserDetailsDto>
{
    public async Task<UserDetailsDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var existingUser = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
            throw new InvalidOperationException($"A user with email '{request.Email}' already exists");

        var user = new User(request.Email, request.FirstName, request.LastName, "pending-hash", request.Role);
        user.SetPasswordHash(passwordHasher.HashPassword(user, request.Password));
        await userRepository.AddAsync(user, cancellationToken);

        return UserMappings.ToDetailsDto(user);
    }
}
