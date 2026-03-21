using FluentValidation;
using MediatR;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Domain.Aggregates;
using TestPlatform.Domain.Events;

namespace TestPlatform.Application.Features.TestSubmissions.SubmitTest;

public sealed class SubmitTestCommandHandler(
    ITestRepository testRepository,
    IQuestionRepository questionRepository,
    ITestAttemptRepository testAttemptRepository,
    IValidator<SubmitTestCommand> validator) : IRequestHandler<SubmitTestCommand, SubmitTestResultDto>
{
    public async Task<SubmitTestResultDto> Handle(SubmitTestCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var test = await testRepository.GetByIdAsync(request.TestId, cancellationToken);
        if (test is null)
            throw new EntityNotFoundException(nameof(Test), request.TestId);

        var questions = await questionRepository.GetByIdsAsync(test.QuestionIds, cancellationToken);
        var questionLookup = questions.ToDictionary(question => question.Id);

        if (questionLookup.Count != test.QuestionIds.Count)
            throw new InvalidOperationException("One or more test questions could not be resolved");

        var attempt = new TestAttempt(request.UserId, test.Id);

        foreach (var answer in request.Answers)
            attempt.AddAnswer(answer.QuestionId, answer.SelectedOptionId);

        attempt.Evaluate(questions);

        var submittedEvent = new TestSubmittedEvent(attempt.Id);
        _ = submittedEvent;

        await testAttemptRepository.AddAsync(attempt, cancellationToken);

        var reviews = attempt.Answers
            .Select(answer =>
            {
                var question = questionLookup[answer.QuestionId];
                var selectedOption = question.Options.Single(option => option.Id == answer.SelectedOptionId);
                var correctOption = question.Options.Single(option => option.Id == question.CorrectOptionId);

                return new SubmitTestQuestionReviewDto(
                    question.Id,
                    question.Text,
                    selectedOption.Id,
                    selectedOption.Text,
                    correctOption.Id,
                    correctOption.Text,
                    selectedOption.Id == correctOption.Id,
                    question.ExplanationText);
            })
            .ToArray();

        return new SubmitTestResultDto(
            attempt.Id,
            attempt.TestId,
            attempt.UserId,
            attempt.Score,
            test.QuestionIds.Count,
            attempt.IsEvaluated,
            reviews);
    }
}
