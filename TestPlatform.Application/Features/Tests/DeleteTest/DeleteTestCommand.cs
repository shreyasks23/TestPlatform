using MediatR;

namespace TestPlatform.Application.Features.Tests.DeleteTest;

public sealed record DeleteTestCommand(Guid TestId, Guid ActorId, bool IsAdmin) : IRequest;
