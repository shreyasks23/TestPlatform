using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Domain.Aggregates;

namespace TestPlatform.Persistence;

internal sealed class InMemoryTestAttemptRepository : ITestAttemptRepository
{
    private readonly List<TestAttempt> _attempts = [];

    public Task AddAsync(TestAttempt attempt, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(attempt);

        _attempts.Add(attempt);
        return Task.CompletedTask;
    }
}
