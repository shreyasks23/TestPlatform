namespace TestPlatform.Application.Features.Auth;

public sealed record AuthTokensDto(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAtUtc);
