using MediatR;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Application.Common.Security;
using TestPlatform.Domain.Aggregates;

namespace TestPlatform.Application.Features.Tests.RemoveQuestionFromTest;

public sealed class RemoveQuestionFromTestCommandHandler(ITestRepository testRepository)
    : IRequestHandler<RemoveQuestionFromTestCommand>
{
    public async Task Handle(RemoveQuestionFromTestCommand request, CancellationToken cancellationToken)
    {
        var test = await testRepository.GetByIdAsync(request.TestId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Test), request.TestId);

        OwnershipAuthorization.EnsureOwnerOrAdmin(test.CreatedByUserId, request.ActorId, request.IsAdmin);
        test.RemoveQuestion(request.QuestionId);
        await testRepository.UpdateAsync(test, cancellationToken);
    }
}
