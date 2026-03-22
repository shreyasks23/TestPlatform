using TestPlatform.Domain.Aggregates;

namespace TestPlatform.Application.Common.Interfaces.Repositories;

public interface IQuestionRepository
{
    Task AddAsync(Question question, CancellationToken cancellationToken);
    Task<IReadOnlyList<Question>> GetAllActiveAsync(CancellationToken cancellationToken);
    Task<Question?> GetByIdAsync(Guid questionId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Question>> GetByIdsAsync(IEnumerable<Guid> questionIds, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid questionId, CancellationToken cancellationToken);
    Task UpdateAsync(Question question, CancellationToken cancellationToken);
}
