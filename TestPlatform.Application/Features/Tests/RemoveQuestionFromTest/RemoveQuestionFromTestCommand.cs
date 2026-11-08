using MediatR;

namespace TestPlatform.Application.Features.Tests.RemoveQuestionFromTest;

public sealed record RemoveQuestionFromTestCommand(
    Guid TestId,
    Guid QuestionId,
    Guid ActorId,
    bool IsAdmin) : IRequest;
