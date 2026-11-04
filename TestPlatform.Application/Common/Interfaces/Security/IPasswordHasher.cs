using TestPlatform.Domain.Entities;

namespace TestPlatform.Application.Common.Interfaces.Security;

public interface IPasswordHasher
{
    string HashPassword(User user, string password);
    bool VerifyPassword(User user, string hashedPassword, string providedPassword);
}
