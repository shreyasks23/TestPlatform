using FluentValidation;
using MediatR;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Domain.Aggregates;

namespace TestPlatform.Application.Features.Tests.CreateTest;

public sealed class CreateTestCommandHandler(
    ITestRepository testRepository,
    IValidator<CreateTestCommand> validator) : IRequestHandler<CreateTestCommand, CreateTestResultDto>
{
    public async Task<CreateTestResultDto> Handle(CreateTestCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var test = new Test(request.Title, request.ActorId);
        await testRepository.AddAsync(test, cancellationToken);

        return new CreateTestResultDto(test.Id, test.Title, test.QuestionIds.Count);
    }
}
