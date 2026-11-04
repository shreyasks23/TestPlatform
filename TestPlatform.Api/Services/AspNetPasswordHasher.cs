using Microsoft.AspNetCore.Identity;
using TestPlatform.Application.Common.Interfaces.Security;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Api.Services;

internal sealed class AspNetPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public string HashPassword(User user, string password)
        => _passwordHasher.HashPassword(user, password);

    public bool VerifyPassword(User user, string hashedPassword, string providedPassword)
        => _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword) != PasswordVerificationResult.Failed;
}
