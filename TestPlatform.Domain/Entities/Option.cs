namespace TestPlatform.Domain.Entities;

public sealed class Option
{
    public Guid Id { get; private set; }
    public string Text { get; private set; } = null!;
    public int Order { get; private set; }

    private Option() { }

    public Option(string text)
    {
        Id = Guid.CreateVersion7();
        Text = ValidateText(text);
    }

    public Option(Guid id, string text)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Option ID cannot be empty", nameof(id));

        Id = id;
        Text = ValidateText(text);
    }

    public void Update(string text, int order)
    {
        if (order < 0)
            throw new ArgumentOutOfRangeException(nameof(order), "Order must be non-negative");

        Text = ValidateText(text);
        Order = order;
    }

    private static string ValidateText(string text)
    {
        return string.IsNullOrWhiteSpace(text)
            ? throw new ArgumentException("Option text cannot be empty", nameof(text))
            : text;
    }
}
