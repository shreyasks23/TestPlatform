using FluentValidation;
using MediatR;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Application.Features.Tests.GetTestById;
using TestPlatform.Domain.Aggregates;

namespace TestPlatform.Application.Features.Tests.AddQuestionToTest;

public sealed class AddQuestionToTestCommandHandler(
    ITestRepository testRepository,
    IQuestionRepository questionRepository,
    IValidator<AddQuestionToTestCommand> validator) : IRequestHandler<AddQuestionToTestCommand, AddQuestionToTestResultDto>
{
    public async Task<AddQuestionToTestResultDto> Handle(AddQuestionToTestCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var test = await testRepository.GetByIdAsync(request.TestId, cancellationToken);
        if (test is null)
            throw new EntityNotFoundException(nameof(Test), request.TestId);

        var question = await questionRepository.GetByIdAsync(request.QuestionId, cancellationToken);
        if (question is null)
            throw new EntityNotFoundException(nameof(Question), request.QuestionId);

        if (question.IsDeleted)
            throw new InvalidOperationException("Deleted questions cannot be attached to tests");

        test.AddQuestion(question.Id);

        return new AddQuestionToTestResultDto(
            test.Id,
            question.Id,
            question.Text,
            question.CorrectOptionId,
            question.ExplanationText,
            question.Options
                .Select(option => new TestQuestionOptionDto(option.Id, option.Text))
                .ToArray());
    }
}
