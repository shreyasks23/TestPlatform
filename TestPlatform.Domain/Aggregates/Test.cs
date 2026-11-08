using TestPlatform.Domain.Entities;

namespace TestPlatform.Domain.Aggregates;

public sealed class Test
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public Guid CreatedByUserId { get; private set; }
    public string Title { get; private set; } = null!;

    private readonly List<TestQuestionReference> _questionReferences = [];
    public IReadOnlyList<TestQuestionReference> QuestionReferences => _questionReferences.AsReadOnly();
    public IReadOnlyList<Guid> QuestionIds => _questionReferences
        .OrderBy(reference => reference.Order)
        .Select(reference => reference.QuestionId)
        .ToArray();

    private Test() { }

    public Test(string title, Guid createdByUserId)
    {
        SetCreator(createdByUserId);
        Title = string.IsNullOrWhiteSpace(title)
            ? throw new ArgumentException("Title cannot be empty", nameof(title))
            : title;
    }

    public Test(Guid id, string title, Guid createdByUserId) : this(title, createdByUserId)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Test ID cannot be empty", nameof(id));

        Id = id;
    }

    public void AddQuestion(Guid questionId)
    {
        if (questionId == Guid.Empty)
            throw new ArgumentException("Question ID cannot be empty", nameof(questionId));

        if (_questionReferences.Any(reference => reference.QuestionId == questionId))
            throw new InvalidOperationException("Question already added to test");

        _questionReferences.Add(new TestQuestionReference(questionId, _questionReferences.Count));
    }

    public void UpdateTitle(string title)
    {
        Title = string.IsNullOrWhiteSpace(title)
            ? throw new ArgumentException("Title cannot be empty", nameof(title))
            : title;
    }

    public void RemoveQuestion(Guid questionId)
    {
        var reference = _questionReferences.SingleOrDefault(item => item.QuestionId == questionId)
            ?? throw new InvalidOperationException("Question is not attached to this test");

        _questionReferences.Remove(reference);
        for (var index = 0; index < _questionReferences.Count; index++)
            _questionReferences[index].UpdateOrder(index);
    }

    private void SetCreator(Guid createdByUserId)
    {
        if (createdByUserId == Guid.Empty)
            throw new ArgumentException("Creator ID cannot be empty", nameof(createdByUserId));

        CreatedByUserId = createdByUserId;
    }
}
