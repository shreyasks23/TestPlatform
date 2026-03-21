namespace TestPlatform.Domain.Aggregates;

public sealed class Test
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public string Title { get; private set; } = null!;

    private readonly List<Guid> _questionIds = [];
    public IReadOnlyList<Guid> QuestionIds => _questionIds.AsReadOnly();

    private Test() { }

    public Test(string title)
    {
        Title = string.IsNullOrWhiteSpace(title)
            ? throw new ArgumentException("Title cannot be empty", nameof(title))
            : title;
    }

    public void AddQuestion(Guid questionId)
    {
        if (questionId == Guid.Empty)
            throw new ArgumentException("Question ID cannot be empty", nameof(questionId));

        if (_questionIds.Contains(questionId))
            throw new InvalidOperationException("Question already added to test");

        _questionIds.Add(questionId);
    }
}
