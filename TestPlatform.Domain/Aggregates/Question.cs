using TestPlatform.Domain.Entities;

namespace TestPlatform.Domain.Aggregates;

public sealed class Question
{
    public Guid Id { get; private set; }
    public Guid CreatedByUserId { get; private set; }
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
        string explanationText,
        Guid createdByUserId)
    {
        Id = Guid.CreateVersion7();
        SetCreator(createdByUserId);
        Update(text, options, correctOptionId, explanationText);
    }

    public Question(
        Guid id,
        string text,
        IReadOnlyCollection<Option> options,
        Guid correctOptionId,
        string explanationText,
        Guid createdByUserId)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Question ID cannot be empty", nameof(id));

        Id = id;
        SetCreator(createdByUserId);
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

        var requestedOptions = options.ToArray();
        if (requestedOptions.Length < 2)
            throw new InvalidOperationException("A question must have at least 2 options");

        if (!requestedOptions.Any(o => o.Id == correctOptionId))
            throw new InvalidOperationException("Correct option must exist");

        var requestedIds = requestedOptions.Select(option => option.Id).ToHashSet();

        _options.RemoveAll(option => !requestedIds.Contains(option.Id));

        for (var index = 0; index < requestedOptions.Length; index++)
        {
            var requestedOption = requestedOptions[index];
            var existingOption = _options.SingleOrDefault(option => option.Id == requestedOption.Id);

            if (existingOption is null)
            {
                requestedOption.Update(requestedOption.Text, index);
                _options.Add(requestedOption);
                continue;
            }

            existingOption.Update(requestedOption.Text, index);
        }

        _options.Sort((left, right) => left.Order.CompareTo(right.Order));
        CorrectOptionId = correctOptionId;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
    }

    private void SetCreator(Guid createdByUserId)
    {
        if (createdByUserId == Guid.Empty)
            throw new ArgumentException("Creator ID cannot be empty", nameof(createdByUserId));

        CreatedByUserId = createdByUserId;
    }
}
