using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TestPlatform.Persistence;

public sealed class TestPlatformDbContextFactory : IDesignTimeDbContextFactory<TestPlatformDbContext>
{
    public TestPlatformDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TestPlatformDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=TestPlatformDb;Username=postgres;Password=postgres");
        return new TestPlatformDbContext(optionsBuilder.Options);
    }
}
