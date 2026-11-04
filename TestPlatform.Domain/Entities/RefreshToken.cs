namespace TestPlatform.Domain.Entities;

public sealed class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset ExpiresAtUtc { get; private set; }
    public DateTimeOffset? RevokedAtUtc { get; private set; }

    private RefreshToken() { }

    public RefreshToken(Guid userId, string tokenHash, DateTimeOffset createdAtUtc, DateTimeOffset expiresAtUtc)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        if (expiresAtUtc <= createdAtUtc)
            throw new ArgumentException("Refresh token expiry must be after creation.", nameof(expiresAtUtc));

        Id = Guid.CreateVersion7();
        UserId = userId;
        TokenHash = string.IsNullOrWhiteSpace(tokenHash)
            ? throw new ArgumentException("Token hash cannot be empty", nameof(tokenHash))
            : tokenHash;
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
    }

    public bool IsActive(DateTimeOffset now)
        => RevokedAtUtc is null && ExpiresAtUtc > now;

    public void Revoke(DateTimeOffset revokedAtUtc)
    {
        if (RevokedAtUtc is not null)
            return;

        RevokedAtUtc = revokedAtUtc;
    }
}
