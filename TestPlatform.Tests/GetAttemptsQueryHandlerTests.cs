using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Application.Features.TestAttempts.GetAttempts;
using TestPlatform.Domain.Aggregates;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Tests;

public sealed class GetAttemptsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsSummaryForTheRequestedUser()
    {
        var userId = Guid.CreateVersion7();
        var testId = Guid.CreateVersion7();
        var attempt = new TestAttempt(userId, testId);
        var options = new[] { new Option("A"), new Option("B") };
        var question = new Question("Question", options, options[0].Id, "Explanation", Guid.CreateVersion7());
        attempt.AddAnswer(question.Id, options[0].Id);
        attempt.Evaluate([question]);

        var repository = new FakeTestAttemptRepository([attempt]);
        var handler = new GetAttemptsQueryHandler(repository);

        var result = await handler.Handle(new GetAttemptsQuery(userId), CancellationToken.None);

        Assert.Equal(userId, repository.RequestedUserId);
        Assert.Collection(
            result,
            summary =>
            {
                Assert.Equal(attempt.Id, summary.AttemptId);
                Assert.Equal(testId, summary.TestId);
                Assert.Equal(1, summary.Score);
                Assert.Equal(1, summary.TotalQuestions);
                Assert.True(summary.IsEvaluated);
            });
    }

    private sealed class FakeTestAttemptRepository(IReadOnlyList<TestAttempt> attempts) : ITestAttemptRepository
    {
        public Guid RequestedUserId { get; private set; }

        public Task AddAsync(TestAttempt attempt, CancellationToken cancellationToken)
            => Task.CompletedTask;

        public Task<IReadOnlyList<TestAttempt>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            RequestedUserId = userId;
            return Task.FromResult(attempts);
        }
    }
}
