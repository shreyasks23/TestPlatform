namespace TestPlatform.Application.Features.TestAttempts.GetAttempts;

public sealed record TestAttemptSummaryDto(
    Guid AttemptId,
    Guid TestId,
    int Score,
    int TotalQuestions,
    bool IsEvaluated);
