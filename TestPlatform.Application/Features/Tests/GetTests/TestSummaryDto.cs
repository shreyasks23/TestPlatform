namespace TestPlatform.Application.Features.Tests.GetTests;

public sealed record TestSummaryDto(
    Guid Id,
    string Title,
    int QuestionCount);
