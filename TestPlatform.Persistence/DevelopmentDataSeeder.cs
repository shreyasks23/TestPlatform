using Microsoft.EntityFrameworkCore;
using TestPlatform.Application.Common.Interfaces.Security;
using TestPlatform.Application.Common.Security;
using TestPlatform.Domain.Aggregates;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Persistence;

public sealed class DevelopmentDataSeeder(
    TestPlatformDbContext dbContext,
    IPasswordHasher passwordHasher)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Users.AnyAsync(cancellationToken) &&
            !await dbContext.Questions.AnyAsync(cancellationToken) &&
            !await dbContext.Tests.AnyAsync(cancellationToken))
        {
            var users = SeedCatalog.CreateSampleUsers(passwordHasher);
            var questions = SeedCatalog.CreateQuestions();
            var tests = SeedCatalog.CreateTests(questions);

            dbContext.Users.AddRange(users);
            dbContext.Questions.AddRange(questions);
            dbContext.Tests.AddRange(tests);
        }

        var existingAdmin = await dbContext.Users.SingleOrDefaultAsync(user => user.Email == SeedCatalog.AdminEmail, cancellationToken);
        if (existingAdmin is null)
        {
            dbContext.Users.Add(SeedCatalog.CreateAdminUser(passwordHasher));
        }
        else
        {
            existingAdmin.SetRole(AuthConstants.Roles.Admin);

            if (string.IsNullOrWhiteSpace(existingAdmin.PasswordHash))
            {
                existingAdmin.SetPasswordHash(passwordHasher.HashPassword(existingAdmin, SeedCatalog.AdminPassword));
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
