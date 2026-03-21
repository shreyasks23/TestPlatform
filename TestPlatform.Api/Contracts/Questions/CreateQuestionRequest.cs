namespace TestPlatform.Api.Contracts.Questions;

public sealed record CreateQuestionRequest(
    string Text,
    IReadOnlyCollection<QuestionOptionRequest> Options,
    int CorrectOptionIndex,
    string ExplanationText);
