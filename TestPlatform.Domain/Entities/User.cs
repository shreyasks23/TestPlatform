using TestPlatform.Domain.Security;

namespace TestPlatform.Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string Role { get; private set; } = null!;
    public int TokenVersion { get; private set; } = 1;

    private User() { }

    public User(string email, string firstName, string lastName, string passwordHash, string role)
    {
        Id = Guid.CreateVersion7();
        Update(email, firstName, lastName);
        SetPasswordHash(passwordHash);
        SetRole(role);
    }

    public User(Guid id, string email, string firstName, string lastName, string passwordHash, string role)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(id));

        Id = id;
        Update(email, firstName, lastName);
        SetPasswordHash(passwordHash);
        SetRole(role);
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

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = string.IsNullOrWhiteSpace(passwordHash)
            ? throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash))
            : passwordHash;
    }

    public void SetRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role) || !UserRoles.IsSupported(role))
            throw new ArgumentException("Role is not supported", nameof(role));

        Role = role;
    }

    public void IncrementTokenVersion()
    {
        if (TokenVersion == int.MaxValue)
            throw new InvalidOperationException("Token version limit has been reached.");

        TokenVersion++;
    }
}
