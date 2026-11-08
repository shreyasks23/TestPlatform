using MediatR;
using TestPlatform.Application.Features.Tests.CreateTest;

namespace TestPlatform.Application.Features.Tests.UpdateTest;

public sealed record UpdateTestCommand(
    Guid TestId,
    string Title,
    Guid ActorId,
    bool IsAdmin) : IRequest<CreateTestResultDto>;
