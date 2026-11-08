using MediatR;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Domain.Aggregates;

namespace TestPlatform.Application.Features.TestAttempts.GetAttemptReview;

public sealed class GetAttemptReviewQueryHandler(
    ITestAttemptRepository testAttemptRepository,
    IQuestionRepository questionRepository,
    ITestRepository testRepository)
    : IRequestHandler<GetAttemptReviewQuery, TestAttemptReviewDto>
{
    public async Task<TestAttemptReviewDto> Handle(
        GetAttemptReviewQuery request,
        CancellationToken cancellationToken)
    {
        var attempt = await testAttemptRepository.GetByIdForUserAsync(
            request.AttemptId,
            request.UserId,
            cancellationToken);
        if (attempt is null)
            throw new EntityNotFoundException(nameof(TestAttempt), request.AttemptId);

        var questions = await questionRepository.GetByIdsAsync(
            attempt.Answers.Select(answer => answer.QuestionId),
            cancellationToken);
        var questionLookup = questions.ToDictionary(question => question.Id);
        var questionReviews = attempt.Answers.Select(answer =>
        {
            if (!questionLookup.TryGetValue(answer.QuestionId, out var question))
                throw new EntityNotFoundException(nameof(Question), answer.QuestionId);

            if (!question.Options.Any(option => option.Id == answer.SelectedOptionId))
                throw new InvalidOperationException(
                    $"Selected option '{answer.SelectedOptionId}' is no longer available for question '{question.Id}'.");

            return new TestAttemptQuestionReviewDto(
                question.Id,
                question.Text,
                answer.SelectedOptionId,
                question.CorrectOptionId,
                question.ExplanationText,
                question.Options
                    .Select(option => new TestAttemptOptionReviewDto(option.Id, option.Text))
                    .ToArray());
        }).ToArray();

        var test = await testRepository.GetByIdAsync(attempt.TestId, cancellationToken);

        return new TestAttemptReviewDto(
            attempt.Id,
            attempt.TestId,
            test?.Title,
            attempt.Score,
            attempt.Answers.Count,
            attempt.IsEvaluated,
            questionReviews);
    }
}
