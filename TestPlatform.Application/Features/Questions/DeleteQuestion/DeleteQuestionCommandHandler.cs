using MediatR;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Application.Common.Security;
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

        OwnershipAuthorization.EnsureOwnerOrAdmin(question.CreatedByUserId, request.ActorId, request.IsAdmin);

        question.SoftDelete();
        await questionRepository.UpdateAsync(question, cancellationToken);
    }
}
