using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TestPlatform.Persistence;

public static class PersistenceApplicationBuilderExtensions
{
    public static async Task InitializePersistenceAsync(this IServiceProvider services, bool isDevelopment)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TestPlatformDbContext>();

        await dbContext.Database.MigrateAsync();

        if (isDevelopment)
        {
            var seeder = scope.ServiceProvider.GetRequiredService<DevelopmentDataSeeder>();
            await seeder.SeedAsync();
        }
    }
}
