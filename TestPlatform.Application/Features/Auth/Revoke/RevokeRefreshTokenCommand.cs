using MediatR;

namespace TestPlatform.Application.Features.Auth.Revoke;

public sealed record RevokeRefreshTokenCommand(string RefreshToken) : IRequest;
