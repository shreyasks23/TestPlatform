using MediatR;

namespace TestPlatform.Application.Features.Auth.LogoutAll;

public sealed record LogoutAllCommand(Guid UserId) : IRequest;
