using FluentValidation;
using MediatR;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Application.Common.Interfaces.Security;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Application.Features.Auth.Login;

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IAccessTokenService accessTokenService,
    IRefreshTokenService refreshTokenService,
    IRefreshTokenRepository refreshTokenRepository,
    IAuthTokenLifetimeProvider tokenLifetimeProvider,
    IDateTimeProvider dateTimeProvider,
    IValidator<LoginCommand> validator) : IRequestHandler<LoginCommand, AuthTokensDto>
{
    public async Task<AuthTokensDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null || !passwordHasher.VerifyPassword(user, user.PasswordHash, request.Password))
            throw new AuthenticationFailedException("Invalid email or password.");

        return await IssueTokensAsync(user, cancellationToken);
    }

    private async Task<AuthTokensDto> IssueTokensAsync(User user, CancellationToken cancellationToken)
    {
        var now = dateTimeProvider.UtcNow;
        var sessionId = Guid.CreateVersion7();
        var accessToken = accessTokenService.CreateToken(user, sessionId);
        var refreshToken = refreshTokenService.GenerateToken();
        var refreshTokenExpiresAtUtc = now.Add(tokenLifetimeProvider.RefreshTokenLifetime);

        await refreshTokenRepository.AddAsync(
            new RefreshToken(user.Id, sessionId, refreshToken.Hash, now, refreshTokenExpiresAtUtc),
            cancellationToken);

        return new AuthTokensDto(
            accessToken.Token,
            accessToken.ExpiresAtUtc,
            refreshToken.Token,
            refreshTokenExpiresAtUtc);
    }
}
