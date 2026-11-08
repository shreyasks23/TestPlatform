namespace TestPlatform.Application.Features.TestAttempts.GetAttemptReview;

public sealed record TestAttemptReviewDto(
    Guid AttemptId,
    Guid TestId,
    string? TestTitle,
    int Score,
    int TotalQuestions,
    bool IsEvaluated,
    IReadOnlyList<TestAttemptQuestionReviewDto> Questions);

public sealed record TestAttemptQuestionReviewDto(
    Guid QuestionId,
    string Text,
    Guid SelectedOptionId,
    Guid CorrectOptionId,
    string ExplanationText,
    IReadOnlyList<TestAttemptOptionReviewDto> Options);

public sealed record TestAttemptOptionReviewDto(Guid Id, string Text);
