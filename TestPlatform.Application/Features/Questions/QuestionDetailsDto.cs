namespace TestPlatform.Application.Features.Questions;

public sealed record QuestionDetailsDto(
    Guid Id,
    string Text,
    string ExplanationText,
    Guid CorrectOptionId,
    bool IsDeleted,
    IReadOnlyCollection<QuestionOptionDto> Options);
