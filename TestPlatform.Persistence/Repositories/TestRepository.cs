using Microsoft.EntityFrameworkCore;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Domain.Aggregates;

namespace TestPlatform.Persistence.Repositories;

internal sealed class TestRepository(TestPlatformDbContext dbContext) : ITestRepository
{
    public async Task AddAsync(Test test, CancellationToken cancellationToken)
    {
        dbContext.Tests.Add(test);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Test>> GetAllAsync(CancellationToken cancellationToken)
        => await dbContext.Tests
            .Include(test => test.QuestionReferences.OrderBy(reference => reference.Order))
            .ToListAsync(cancellationToken);

    public async Task<Test?> GetByIdAsync(Guid testId, CancellationToken cancellationToken)
        => await dbContext.Tests
            .Include(test => test.QuestionReferences.OrderBy(reference => reference.Order))
            .SingleOrDefaultAsync(test => test.Id == testId, cancellationToken);

    public Task UpdateAsync(Test test, CancellationToken cancellationToken)
        => dbContext.SaveChangesAsync(cancellationToken);
}
