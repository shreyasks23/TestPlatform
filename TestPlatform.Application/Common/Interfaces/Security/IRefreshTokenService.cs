using TestPlatform.Application.Common.Models;

namespace TestPlatform.Application.Common.Interfaces.Security;

public interface IRefreshTokenService
{
    RefreshTokenMaterial GenerateToken();
    string HashToken(string token);
}
