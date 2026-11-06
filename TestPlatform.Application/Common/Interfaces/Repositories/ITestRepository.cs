using TestPlatform.Domain.Aggregates;

namespace TestPlatform.Application.Common.Interfaces.Repositories;

public interface ITestRepository
{
    Task AddAsync(Test test, CancellationToken cancellationToken);
    Task<IReadOnlyList<Test>> GetAllAsync(CancellationToken cancellationToken);
    Task<Test?> GetByIdAsync(Guid testId, CancellationToken cancellationToken);
    Task UpdateAsync(Test test, CancellationToken cancellationToken);
    Task DeleteAsync(Test test, CancellationToken cancellationToken);
}
