namespace TestPlatform.Application.Features.TestSubmissions.SubmitTest;

public sealed record SubmitTestQuestionReviewDto(
    Guid QuestionId,
    string Text,
    Guid SelectedOptionId,
    string SelectedOptionText,
    Guid CorrectOptionId,
    string CorrectOptionText,
    bool IsCorrect,
    string ExplanationText);
