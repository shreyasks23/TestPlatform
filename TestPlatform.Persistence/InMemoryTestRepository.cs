using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Domain.Aggregates;

namespace TestPlatform.Persistence;

internal sealed class InMemoryTestRepository(InMemoryStore store) : ITestRepository
{
    public Task AddAsync(Test test, CancellationToken cancellationToken)
    {
        store.AddTest(test);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Test>> GetAllAsync(CancellationToken cancellationToken)
        => Task.FromResult(store.Tests);

    public Task<Test?> GetByIdAsync(Guid testId, CancellationToken cancellationToken)
        => Task.FromResult(store.Tests.SingleOrDefault(candidate => candidate.Id == testId));
}
