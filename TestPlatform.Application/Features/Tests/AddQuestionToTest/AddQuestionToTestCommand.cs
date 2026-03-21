using MediatR;

namespace TestPlatform.Application.Features.Tests.AddQuestionToTest;

public sealed record AddQuestionToTestCommand(
    Guid TestId,
    Guid QuestionId) : IRequest<AddQuestionToTestResultDto>;
