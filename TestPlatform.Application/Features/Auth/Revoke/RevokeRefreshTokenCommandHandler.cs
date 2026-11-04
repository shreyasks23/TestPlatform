using FluentValidation;
using MediatR;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Application.Common.Interfaces.Security;

namespace TestPlatform.Application.Features.Auth.Revoke;

public sealed class RevokeRefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IRefreshTokenService refreshTokenService,
    IDateTimeProvider dateTimeProvider,
    IValidator<RevokeRefreshTokenCommand> validator) : IRequestHandler<RevokeRefreshTokenCommand>
{
    public async Task Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var tokenHash = refreshTokenService.HashToken(request.RefreshToken);
        var existingRefreshToken = await refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);
        if (existingRefreshToken is null || existingRefreshToken.RevokedAtUtc is not null)
            return;

        existingRefreshToken.Revoke(dateTimeProvider.UtcNow);
        await refreshTokenRepository.UpdateAsync(existingRefreshToken, cancellationToken);
    }
}
