using System.Security.Cryptography;
using System.Text;
using TestPlatform.Application.Common.Interfaces.Security;
using TestPlatform.Application.Common.Models;

namespace TestPlatform.Api.Services;

internal sealed class RefreshTokenService : IRefreshTokenService
{
    public RefreshTokenMaterial GenerateToken()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        return new RefreshTokenMaterial(token, HashToken(token));
    }

    public string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
