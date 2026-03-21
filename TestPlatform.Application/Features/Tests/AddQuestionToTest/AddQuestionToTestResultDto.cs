using TestPlatform.Application.Features.Tests.GetTestById;

namespace TestPlatform.Application.Features.Tests.AddQuestionToTest;

public sealed record AddQuestionToTestResultDto(
    Guid TestId,
    Guid QuestionId,
    string Text,
    Guid CorrectOptionId,
    string ExplanationText,
    IReadOnlyCollection<TestQuestionOptionDto> Options);
