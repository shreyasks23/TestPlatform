using Microsoft.EntityFrameworkCore;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Domain.Aggregates;

namespace TestPlatform.Persistence.Repositories;

internal sealed class QuestionRepository(TestPlatformDbContext dbContext) : IQuestionRepository
{
    public async Task AddAsync(Question question, CancellationToken cancellationToken)
    {
        dbContext.Questions.Add(question);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid questionId, CancellationToken cancellationToken)
        => dbContext.Questions.AnyAsync(question => question.Id == questionId, cancellationToken);

    public async Task<IReadOnlyList<Question>> GetAllActiveAsync(CancellationToken cancellationToken)
        => await dbContext.Questions
            .Where(question => !question.IsDeleted)
            .Include(question => question.Options.OrderBy(option => option.Order))
            .ToListAsync(cancellationToken);

    public async Task<Question?> GetByIdAsync(Guid questionId, CancellationToken cancellationToken)
        => await dbContext.Questions
            .Include(question => question.Options.OrderBy(option => option.Order))
            .SingleOrDefaultAsync(question => question.Id == questionId, cancellationToken);

    public async Task<IReadOnlyList<Question>> GetByIdsAsync(IEnumerable<Guid> questionIds, CancellationToken cancellationToken)
    {
        var ids = questionIds.Distinct().ToArray();

        return await dbContext.Questions
            .Where(question => ids.Contains(question.Id))
            .Include(question => question.Options.OrderBy(option => option.Order))
            .ToListAsync(cancellationToken);
    }

    public Task UpdateAsync(Question question, CancellationToken cancellationToken)
        => dbContext.SaveChangesAsync(cancellationToken);
}
