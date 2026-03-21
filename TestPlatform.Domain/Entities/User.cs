namespace TestPlatform.Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;

    private User() { }

    public User(string email)
    {
        Id = Guid.CreateVersion7();
        Email = string.IsNullOrWhiteSpace(email)
            ? throw new ArgumentException("Email cannot be empty", nameof(email))
            : email;
    }
}
