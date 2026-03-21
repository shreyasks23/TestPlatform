namespace TestPlatform.Application.Features.Tests.GetTestById;

public sealed record TestQuestionDto(
    Guid Id,
    string Text,
    IReadOnlyCollection<TestQuestionOptionDto> Options);
