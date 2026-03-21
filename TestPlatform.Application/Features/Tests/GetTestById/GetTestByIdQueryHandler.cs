using MediatR;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Domain.Aggregates;

namespace TestPlatform.Application.Features.Tests.GetTestById;

public sealed class GetTestByIdQueryHandler(
    ITestRepository testRepository,
    IQuestionRepository questionRepository)
    : IRequestHandler<GetTestByIdQuery, TestDetailsDto>
{
    public async Task<TestDetailsDto> Handle(GetTestByIdQuery request, CancellationToken cancellationToken)
    {
        var test = await testRepository.GetByIdAsync(request.TestId, cancellationToken);
        if (test is null)
            throw new EntityNotFoundException(nameof(Test), request.TestId);

        var questions = await questionRepository.GetByIdsAsync(test.QuestionIds, cancellationToken);
        var questionLookup = questions.ToDictionary(question => question.Id);

        return new TestDetailsDto(
            test.Id,
            test.Title,
            test.QuestionIds
                .Select(questionId =>
                {
                    if (!questionLookup.TryGetValue(questionId, out var question))
                        throw new InvalidOperationException($"Question '{questionId}' could not be resolved for test '{test.Id}'");

                    return new TestQuestionDto(
                        question.Id,
                        question.Text,
                        question.Options
                            .Select(option => new TestQuestionOptionDto(option.Id, option.Text))
                            .ToArray());
                })
                .ToArray());
    }
}
