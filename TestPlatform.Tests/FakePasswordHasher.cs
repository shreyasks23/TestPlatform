using TestPlatform.Application.Common.Interfaces.Security;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Tests;

internal sealed class FakePasswordHasher : IPasswordHasher
{
    public string HashPassword(User user, string password)
        => $"hashed::{password}";

    public bool VerifyPassword(User user, string hashedPassword, string providedPassword)
        => hashedPassword == HashPassword(user, providedPassword);
}
