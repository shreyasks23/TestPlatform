namespace TestPlatform.Application.Features.TestSubmissions.SubmitTest;

public sealed record SubmitTestResultDto(
    Guid AttemptId,
    Guid TestId,
    Guid UserId,
    int Score,
    int TotalQuestions,
    bool IsEvaluated,
    IReadOnlyCollection<SubmitTestQuestionReviewDto> Questions);
