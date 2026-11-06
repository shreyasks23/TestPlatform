using FluentValidation;
using MediatR;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Application.Common.Interfaces.Security;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Application.Features.Auth.Refresh;

public sealed class RefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    IRefreshTokenService refreshTokenService,
    IAccessTokenService accessTokenService,
    IAuthTokenLifetimeProvider tokenLifetimeProvider,
    IDateTimeProvider dateTimeProvider,
    IValidator<RefreshTokenCommand> validator) : IRequestHandler<RefreshTokenCommand, AuthTokensDto>
{
    public async Task<AuthTokensDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var now = dateTimeProvider.UtcNow;
        var tokenHash = refreshTokenService.HashToken(request.RefreshToken);
        var existingRefreshToken = await refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (existingRefreshToken is null || !existingRefreshToken.IsActive(now))
            throw new AuthenticationFailedException("Invalid refresh token.");

        var user = await userRepository.GetByIdAsync(existingRefreshToken.UserId, cancellationToken);
        if (user is null)
            throw new AuthenticationFailedException("Invalid refresh token.");

        existingRefreshToken.Revoke(now);
        await refreshTokenRepository.UpdateAsync(existingRefreshToken, cancellationToken);

        var newRefreshToken = refreshTokenService.GenerateToken();
        var refreshTokenExpiresAtUtc = now.Add(tokenLifetimeProvider.RefreshTokenLifetime);

        await refreshTokenRepository.AddAsync(
            new RefreshToken(user.Id, existingRefreshToken.SessionId, newRefreshToken.Hash, now, refreshTokenExpiresAtUtc),
            cancellationToken);

        var accessToken = accessTokenService.CreateToken(user, existingRefreshToken.SessionId);

        return new AuthTokensDto(
            accessToken.Token,
            accessToken.ExpiresAtUtc,
            newRefreshToken.Token,
            refreshTokenExpiresAtUtc);
    }
}
