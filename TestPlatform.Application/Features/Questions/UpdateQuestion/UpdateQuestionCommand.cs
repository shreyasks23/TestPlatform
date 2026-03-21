using MediatR;
using TestPlatform.Application.Features.Questions;

namespace TestPlatform.Application.Features.Questions.UpdateQuestion;

public sealed record UpdateQuestionCommand(
    Guid QuestionId,
    string Text,
    IReadOnlyCollection<QuestionOptionInputDto> Options,
    int CorrectOptionIndex,
    string ExplanationText) : IRequest<QuestionDetailsDto>;
