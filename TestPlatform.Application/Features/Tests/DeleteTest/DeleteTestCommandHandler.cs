using MediatR;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Application.Common.Security;
using TestPlatform.Domain.Aggregates;

namespace TestPlatform.Application.Features.Tests.DeleteTest;

public sealed class DeleteTestCommandHandler(ITestRepository testRepository) : IRequestHandler<DeleteTestCommand>
{
    public async Task Handle(DeleteTestCommand request, CancellationToken cancellationToken)
    {
        var test = await testRepository.GetByIdAsync(request.TestId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Test), request.TestId);

        OwnershipAuthorization.EnsureOwnerOrAdmin(test.CreatedByUserId, request.ActorId, request.IsAdmin);
        await testRepository.DeleteAsync(test, cancellationToken);
    }
}
