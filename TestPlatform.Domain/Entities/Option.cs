namespace TestPlatform.Domain.Entities;

public sealed class Option
{
    public Guid Id { get; private set; }
    public string Text { get; private set; } = null!;

    private Option() { }

    public Option(string text)
    {
        Id = Guid.CreateVersion7();
        Text = string.IsNullOrWhiteSpace(text)
            ? throw new ArgumentException("Option text cannot be empty", nameof(text))
            : text;
    }
}
