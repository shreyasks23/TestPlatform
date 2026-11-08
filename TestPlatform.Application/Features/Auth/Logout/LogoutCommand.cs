using MediatR;

namespace TestPlatform.Application.Features.Auth.Logout;

public sealed record LogoutCommand(Guid UserId, Guid SessionId) : IRequest;
