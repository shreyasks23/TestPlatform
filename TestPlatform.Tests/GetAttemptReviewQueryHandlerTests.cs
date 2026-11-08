using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Application.Features.TestAttempts.GetAttemptReview;
using TestPlatform.Domain.Aggregates;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Tests;

public sealed class GetAttemptReviewQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsQuestionOptionsAndAnswerExplanationForOwnedAttempt()
    {
        var userId = Guid.CreateVersion7();
        var ownerId = Guid.CreateVersion7();
        var options = new[] { new Option("Incorrect"), new Option("Correct") };
        var question = new Question("Question text", options, options[1].Id, "Because it is correct.", ownerId);
        var attempt = new TestAttempt(userId, Guid.CreateVersion7());
        attempt.AddAnswer(question.Id, options[0].Id);
        attempt.Evaluate([question]);

        var handler = new GetAttemptReviewQueryHandler(
            new FakeTestAttemptRepository(attempt),
            new QuestionRepositoryStub([question]),
            new TestRepositoryStub([]));

        var result = await handler.Handle(
            new GetAttemptReviewQuery(attempt.Id, userId),
            CancellationToken.None);

        Assert.Equal(attempt.Id, result.AttemptId);
        Assert.Equal(0, result.Score);
        var review = Assert.Single(result.Questions);
        Assert.Equal(question.Id, review.QuestionId);
        Assert.Equal(options[0].Id, review.SelectedOptionId);
        Assert.Equal(options[1].Id, review.CorrectOptionId);
        Assert.Equal("Because it is correct.", review.ExplanationText);
        Assert.Equal(options.Select(option => option.Id), review.Options.Select(option => option.Id));
    }

    [Fact]
    public async Task Handle_WhenAttemptBelongsToAnotherUser_ThrowsNotFound()
    {
        var attempt = new TestAttempt(Guid.CreateVersion7(), Guid.CreateVersion7());
        var handler = new GetAttemptReviewQueryHandler(
            new FakeTestAttemptRepository(attempt),
            new QuestionRepositoryStub([]),
            new TestRepositoryStub([]));

        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            handler.Handle(
                new GetAttemptReviewQuery(attempt.Id, Guid.CreateVersion7()),
                CancellationToken.None));
    }

    private sealed class FakeTestAttemptRepository(TestAttempt attempt) : ITestAttemptRepository
    {
        public Task AddAsync(TestAttempt testAttempt, CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task<IReadOnlyList<TestAttempt>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<TestAttempt>>([]);

        public Task<TestAttempt?> GetByIdForUserAsync(Guid attemptId, Guid userId, CancellationToken cancellationToken)
            => Task.FromResult<TestAttempt?>(
                attempt.Id == attemptId && attempt.UserId == userId ? attempt : null);
    }
}
