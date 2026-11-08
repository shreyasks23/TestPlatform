namespace TestPlatform.Application.Features.Tests.GetTests;

public sealed record TestSummaryDto(
    Guid Id,
    Guid CreatedByUserId,
    string Title,
    int QuestionCount);
