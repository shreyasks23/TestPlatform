using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TestPlatform.Persistence;

public sealed class TestPlatformDbContextFactory : IDesignTimeDbContextFactory<TestPlatformDbContext>
{
    public TestPlatformDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__TestPlatformDatabase")
            ?? throw new InvalidOperationException(
                "Set ConnectionStrings__TestPlatformDatabase before running EF Core design-time commands.");

        var optionsBuilder = new DbContextOptionsBuilder<TestPlatformDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new TestPlatformDbContext(optionsBuilder.Options);
    }
}
