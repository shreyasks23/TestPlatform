using MediatR;

namespace TestPlatform.Application.Features.Tests.CreateTest;

public sealed record CreateTestCommand(string Title, Guid ActorId) : IRequest<CreateTestResultDto>;
