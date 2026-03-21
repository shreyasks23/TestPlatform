namespace TestPlatform.Application.Features.Tests.GetTestById;

public sealed record TestDetailsDto(
    Guid Id,
    string Title,
    IReadOnlyCollection<TestQuestionDto> Questions);
