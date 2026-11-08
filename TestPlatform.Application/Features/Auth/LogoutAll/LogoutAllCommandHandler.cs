using FluentValidation;
using MediatR;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Application.Common.Interfaces.Security;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Application.Features.Auth.LogoutAll;

public sealed class LogoutAllCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IDateTimeProvider dateTimeProvider,
    IValidator<LogoutAllCommand> validator) : IRequestHandler<LogoutAllCommand>
{
    public async Task Handle(LogoutAllCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new AuthenticationFailedException("The authenticated user no longer exists.");

        var now = dateTimeProvider.UtcNow;
        user.IncrementTokenVersion();
        await userRepository.UpdateAsync(user, cancellationToken);
        await refreshTokenRepository.RevokeAllForUserAsync(user.Id, now, cancellationToken);
    }
}
