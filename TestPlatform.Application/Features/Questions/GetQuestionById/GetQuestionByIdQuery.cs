using MediatR;
using TestPlatform.Application.Features.Questions;

namespace TestPlatform.Application.Features.Questions.GetQuestionById;

public sealed record GetQuestionByIdQuery(Guid QuestionId) : IRequest<QuestionDetailsDto>;
