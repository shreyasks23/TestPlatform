using MediatR;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Domain.Aggregates;

namespace TestPlatform.Application.Features.Questions.DeleteQuestion;

public sealed class DeleteQuestionCommandHandler(IQuestionRepository questionRepository)
    : IRequestHandler<DeleteQuestionCommand>
{
    public async Task Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await questionRepository.GetByIdAsync(request.QuestionId, cancellationToken);
        if (question is null)
            throw new EntityNotFoundException(nameof(Question), request.QuestionId);

        question.SoftDelete();
        await questionRepository.UpdateAsync(question, cancellationToken);
    }
}
