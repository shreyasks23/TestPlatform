using Microsoft.Extensions.DependencyInjection;
using TestPlatform.Application.Common.Interfaces.Repositories;

namespace TestPlatform.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryStore>();
        services.AddSingleton<IQuestionRepository, InMemoryQuestionRepository>();
        services.AddSingleton<ITestRepository, InMemoryTestRepository>();
        services.AddSingleton<ITestAttemptRepository, InMemoryTestAttemptRepository>();

        return services;
    }
}
