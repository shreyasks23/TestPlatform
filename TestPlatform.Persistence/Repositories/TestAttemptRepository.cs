using Microsoft.EntityFrameworkCore;
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

    public async Task<IReadOnlyList<TestAttempt>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        => await dbContext.TestAttempts
            .Where(attempt => attempt.UserId == userId)
            .OrderByDescending(attempt => attempt.Id)
            .ToListAsync(cancellationToken);
}
