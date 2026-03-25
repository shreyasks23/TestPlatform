using FluentValidation;
using MediatR;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Application.Features.Users;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Application.Features.Users.CreateUser;

public sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IValidator<CreateUserCommand> validator) : IRequestHandler<CreateUserCommand, UserDetailsDto>
{
    public async Task<UserDetailsDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var existingUser = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
            throw new InvalidOperationException($"A user with email '{request.Email}' already exists");

        var user = new User(request.Email, request.FirstName, request.LastName);
        await userRepository.AddAsync(user, cancellationToken);

        return UserMappings.ToDetailsDto(user);
    }
}
