namespace TestPlatform.Application.Features.Tests.CreateTest;

public sealed record CreateTestResultDto(
    Guid Id,
    string Title,
    int QuestionCount);
