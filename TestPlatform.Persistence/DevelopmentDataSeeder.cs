using Microsoft.EntityFrameworkCore;
using TestPlatform.Domain.Aggregates;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Persistence;

public sealed class DevelopmentDataSeeder(TestPlatformDbContext dbContext)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await dbContext.Users.AnyAsync(cancellationToken) ||
            await dbContext.Questions.AnyAsync(cancellationToken) ||
            await dbContext.Tests.AnyAsync(cancellationToken))
        {
            return;
        }

        var users = SeedCatalog.CreateUsers();
        var questions = SeedCatalog.CreateQuestions();
        var tests = SeedCatalog.CreateTests(questions);

        dbContext.Users.AddRange(users);
        dbContext.Questions.AddRange(questions);
        dbContext.Tests.AddRange(tests);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
