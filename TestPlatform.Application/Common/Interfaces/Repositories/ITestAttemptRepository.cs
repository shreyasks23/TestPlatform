using TestPlatform.Domain.Aggregates;

namespace TestPlatform.Application.Common.Interfaces.Repositories;

public interface ITestAttemptRepository
{
    Task AddAsync(TestAttempt attempt, CancellationToken cancellationToken);
}
