using FluentValidation;
using MediatR;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Application.Features.Users;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Application.Features.Users.UpdateUser;

public sealed class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IValidator<UpdateUserCommand> validator) : IRequestHandler<UpdateUserCommand, UserDetailsDto>
{
    public async Task<UserDetailsDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            throw new EntityNotFoundException(nameof(User), request.UserId);

        var existingUser = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null && existingUser.Id != request.UserId)
            throw new InvalidOperationException($"A user with email '{request.Email}' already exists");

        user.Update(request.Email, request.FirstName, request.LastName);
        await userRepository.UpdateAsync(user, cancellationToken);

        return UserMappings.ToDetailsDto(user);
    }
}
