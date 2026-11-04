using MediatR;

namespace TestPlatform.Application.Features.Auth.Refresh;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<AuthTokensDto>;
