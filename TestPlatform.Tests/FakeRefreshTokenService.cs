using TestPlatform.Application.Common.Interfaces.Security;
using TestPlatform.Application.Common.Models;

namespace TestPlatform.Tests;

internal sealed class FakeRefreshTokenService : IRefreshTokenService
{
    private int _sequence = 1;

    public RefreshTokenMaterial GenerateToken()
    {
        var token = $"refresh-token-{_sequence++}";
        return new RefreshTokenMaterial(token, HashToken(token));
    }

    public string HashToken(string token)
        => $"hash::{token}";
}
