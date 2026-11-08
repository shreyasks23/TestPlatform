using FluentValidation;
using MediatR;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Application.Common.Security;
using TestPlatform.Application.Features.Tests.CreateTest;
using TestPlatform.Domain.Aggregates;

namespace TestPlatform.Application.Features.Tests.UpdateTest;

public sealed class UpdateTestCommandHandler(
    ITestRepository testRepository,
    IValidator<UpdateTestCommand> validator) : IRequestHandler<UpdateTestCommand, CreateTestResultDto>
{
    public async Task<CreateTestResultDto> Handle(UpdateTestCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var test = await testRepository.GetByIdAsync(request.TestId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Test), request.TestId);

        OwnershipAuthorization.EnsureOwnerOrAdmin(test.CreatedByUserId, request.ActorId, request.IsAdmin);
        test.UpdateTitle(request.Title);
        await testRepository.UpdateAsync(test, cancellationToken);
        return new CreateTestResultDto(test.Id, test.Title, test.QuestionIds.Count);
    }
}
