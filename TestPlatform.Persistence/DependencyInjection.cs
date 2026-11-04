using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Persistence.Repositories;

namespace TestPlatform.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TestPlatformDatabase")
            ?? throw new InvalidOperationException("Connection string 'TestPlatformDatabase' was not found.");

        services.AddDbContext<TestPlatformDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ITestRepository, TestRepository>();
        services.AddScoped<ITestAttemptRepository, TestAttemptRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<DevelopmentDataSeeder>();

        return services;
    }
}
