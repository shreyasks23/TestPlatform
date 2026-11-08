using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TestPlatform.Persistence;

public static class PersistenceApplicationBuilderExtensions
{
    public static async Task InitializePersistenceAsync(this IServiceProvider services, bool isDevelopment)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TestPlatformDbContext>();

        if (!isDevelopment)
            return;

        await dbContext.Database.MigrateAsync();

        var seeder = scope.ServiceProvider.GetRequiredService<DevelopmentDataSeeder>();
        await seeder.SeedAsync();
    }

    public static async Task MigrateDatabaseAsync(this IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TestPlatformDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
