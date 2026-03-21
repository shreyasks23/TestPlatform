using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Domain.Aggregates;

namespace TestPlatform.Persistence;

internal sealed class InMemoryQuestionRepository(InMemoryStore store) : IQuestionRepository
{
    public Task AddAsync(Question question, CancellationToken cancellationToken)
    {
        store.AddQuestion(question);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Question>> GetAllActiveAsync(CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<Question>>(store.Questions.Where(question => !question.IsDeleted).ToArray());

    public Task<Question?> GetByIdAsync(Guid questionId, CancellationToken cancellationToken)
        => Task.FromResult(store.Questions.SingleOrDefault(question => question.Id == questionId));

    public Task<IReadOnlyList<Question>> GetByIdsAsync(IEnumerable<Guid> questionIds, CancellationToken cancellationToken)
    {
        var ids = questionIds.Distinct().ToHashSet();
        return Task.FromResult<IReadOnlyList<Question>>(store.Questions.Where(question => ids.Contains(question.Id)).ToArray());
    }

    public Task UpdateAsync(Question question, CancellationToken cancellationToken)
        => Task.CompletedTask;
}
