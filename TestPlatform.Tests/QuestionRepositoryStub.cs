using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Domain.Aggregates;

namespace TestPlatform.Tests;

internal sealed class QuestionRepositoryStub(params IEnumerable<Question> seededQuestions) : IQuestionRepository
{
    private readonly List<Question> _questions = [.. seededQuestions];

    public Question? AddedQuestion { get; private set; }

    public Task AddAsync(Question question, CancellationToken cancellationToken)
    {
        AddedQuestion = question;
        _questions.Add(question);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Question>> GetAllActiveAsync(CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<Question>>(_questions.Where(question => !question.IsDeleted).ToArray());

    public Task<Question?> GetByIdAsync(Guid questionId, CancellationToken cancellationToken)
        => Task.FromResult(_questions.SingleOrDefault(question => question.Id == questionId));

    public Task<IReadOnlyList<Question>> GetByIdsAsync(IEnumerable<Guid> questionIds, CancellationToken cancellationToken)
    {
        var ids = questionIds.ToHashSet();
        return Task.FromResult<IReadOnlyList<Question>>(_questions.Where(question => ids.Contains(question.Id)).ToArray());
    }

    public Task UpdateAsync(Question question, CancellationToken cancellationToken)
        => Task.CompletedTask;
}
