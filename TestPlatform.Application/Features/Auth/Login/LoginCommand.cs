using MediatR;

namespace TestPlatform.Application.Features.Auth.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<AuthTokensDto>;
