using TestPlatform.Persistence;

namespace TestPlatform.Api.Endpoints;

internal static class HealthEndpoints
{
    public static IEndpointRouteBuilder MapHealthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health", CheckDatabaseAsync)
            .AllowAnonymous();
        app.MapGet("/healthz", CheckDatabaseAsync)
            .AllowAnonymous();

        return app;
    }

    private static async Task<IResult> CheckDatabaseAsync(
        TestPlatformDbContext dbContext,
        CancellationToken cancellationToken) =>
        await dbContext.Database.CanConnectAsync(cancellationToken)
            ? Results.Ok()
            : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
}
