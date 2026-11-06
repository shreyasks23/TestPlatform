using MediatR;
using TestPlatform.Application.Features.Questions;

namespace TestPlatform.Application.Features.Questions.CreateQuestion;

public sealed record CreateQuestionCommand(
    string Text,
    IReadOnlyCollection<QuestionOptionInputDto> Options,
    int CorrectOptionIndex,
    string ExplanationText,
    Guid ActorId) : IRequest<QuestionDetailsDto>;
