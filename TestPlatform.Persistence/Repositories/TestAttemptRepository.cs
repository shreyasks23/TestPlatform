using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Domain.Aggregates;

namespace TestPlatform.Persistence.Repositories;

internal sealed class TestAttemptRepository(TestPlatformDbContext dbContext) : ITestAttemptRepository
{
    public async Task AddAsync(TestAttempt attempt, CancellationToken cancellationToken)
    {
        dbContext.TestAttempts.Add(attempt);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
