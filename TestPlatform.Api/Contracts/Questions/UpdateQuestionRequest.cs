namespace TestPlatform.Api.Contracts.Questions;

public sealed record UpdateQuestionRequest(
    string Text,
    IReadOnlyCollection<QuestionOptionRequest> Options,
    int CorrectOptionIndex,
    string ExplanationText);
