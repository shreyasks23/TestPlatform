using TestPlatform.Domain.Entities;

namespace TestPlatform.Domain.Aggregates;

public sealed class Question
{
    public Guid Id { get; private set; }
    public string Text { get; private set; } = null!;
    public string ExplanationText { get; private set; } = null!;
    public bool IsDeleted { get; private set; }

    private readonly List<Option> _options = [];
    public IReadOnlyList<Option> Options => _options.AsReadOnly();

    public Guid CorrectOptionId { get; private set; }

    private Question() { }

    public Question(
        string text,
        IReadOnlyCollection<Option> options,
        Guid correctOptionId,
        string explanationText)
    {
        Id = Guid.CreateVersion7();
        Update(text, options, correctOptionId, explanationText);
    }

    public void Update(
        string text,
        IReadOnlyCollection<Option> options,
        Guid correctOptionId,
        string explanationText)
    {
        ArgumentNullException.ThrowIfNull(options);

        Text = string.IsNullOrWhiteSpace(text)
            ? throw new ArgumentException("Question text cannot be empty", nameof(text))
            : text;

        ExplanationText = string.IsNullOrWhiteSpace(explanationText)
            ? throw new ArgumentException("Explanation text cannot be empty", nameof(explanationText))
            : explanationText;

        _options.Clear();
        _options.AddRange(options);

        if (_options.Count < 2)
            throw new InvalidOperationException("A question must have at least 2 options");

        if (!_options.Any(o => o.Id == correctOptionId))
            throw new InvalidOperationException("Correct option must exist");

        CorrectOptionId = correctOptionId;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }
}
