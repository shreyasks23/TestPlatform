using MediatR;
using TestPlatform.Application.Features.Questions;

namespace TestPlatform.Application.Features.Questions.GetQuestions;

public sealed record GetQuestionsQuery : IRequest<IReadOnlyList<QuestionDetailsDto>>;
