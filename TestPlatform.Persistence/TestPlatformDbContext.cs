using Microsoft.EntityFrameworkCore;
using TestPlatform.Domain.Aggregates;
using TestPlatform.Domain.Entities;
using TestPlatform.Persistence.Configurations;

namespace TestPlatform.Persistence;

public sealed class TestPlatformDbContext(DbContextOptions<TestPlatformDbContext> options) : DbContext(options)
{
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Test> Tests => Set<Test>();
    public DbSet<TestAttempt> TestAttempts => Set<TestAttempt>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new QuestionConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        modelBuilder.ApplyConfiguration(new TestConfiguration());
        modelBuilder.ApplyConfiguration(new TestAttemptConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
