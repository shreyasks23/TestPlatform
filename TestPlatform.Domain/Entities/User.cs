namespace TestPlatform.Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;

    private User() { }

    public User(string email, string firstName, string lastName)
    {
        Id = Guid.CreateVersion7();
        Update(email, firstName, lastName);
    }

    public User(Guid id, string email, string firstName, string lastName)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(id));

        Id = id;
        Update(email, firstName, lastName);
    }

    public void Update(string email, string firstName, string lastName)
    {
        Email = string.IsNullOrWhiteSpace(email)
            ? throw new ArgumentException("Email cannot be empty", nameof(email))
            : email;

        FirstName = string.IsNullOrWhiteSpace(firstName)
            ? throw new ArgumentException("First name cannot be empty", nameof(firstName))
            : firstName;

        LastName = string.IsNullOrWhiteSpace(lastName)
            ? throw new ArgumentException("Last name cannot be empty", nameof(lastName))
            : lastName;
    }
}
