using MediatR;

namespace TestPlatform.Application.Features.Questions.DeleteQuestion;

public sealed record DeleteQuestionCommand(Guid QuestionId) : IRequest;
