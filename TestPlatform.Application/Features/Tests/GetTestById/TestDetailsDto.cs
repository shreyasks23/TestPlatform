namespace TestPlatform.Application.Features.Tests.GetTestById;

public sealed record TestDetailsDto(
    Guid Id,
    Guid CreatedByUserId,
    string Title,
    IReadOnlyCollection<TestQuestionDto> Questions);
