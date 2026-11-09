using Microsoft.AspNetCore.Mvc.Testing;

namespace TestPlatform.Tests;

public sealed class HealthEndpointsTests : IClassFixture<HealthEndpointsTests.ApiFactory>
{
    private readonly HttpClient _client;

    public HealthEndpointsTests(ApiFactory factory)
    {
        _client = factory.CreateConfiguredClient();
    }

    [Theory]
    [InlineData("/health")]
    [InlineData("/healthz")]
    public async Task HealthRoutes_ReturnUnavailableWhenDatabaseCannotConnect(string path)
    {
        var response = await _client.GetAsync(path);

        Assert.Equal(System.Net.HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task Cors_AllowsConfiguredOrigin()
    {
        using var request = new HttpRequestMessage(HttpMethod.Options, "/health");
        request.Headers.Add("Origin", "https://client.example");
        request.Headers.Add("Access-Control-Request-Method", "GET");

        var response = await _client.SendAsync(request);

        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal("https://client.example", response.Headers.GetValues("Access-Control-Allow-Origin").Single());
    }

    public sealed class ApiFactory : WebApplicationFactory<Program>
    {
        public HttpClient CreateConfiguredClient()
        {
            var settings = new Dictionary<string, string?>
            {
                ["ASPNETCORE_ENVIRONMENT"] = "Production",
                ["ConnectionStrings__TestPlatformDatabase"] = "Host=127.0.0.1;Port=1;Database=TestPlatformDb;Username=postgres;Password=unused;Timeout=1",
                ["Jwt__Issuer"] = "TestPlatform.Api",
                ["Jwt__Audience"] = "TestPlatform.Clients",
                ["Jwt__SigningKey"] = "HealthEndpointsTests.SigningKey.OnlyForTesting.2026",
                ["Cors__AllowedOrigins__0"] = "https://client.example"
            };
            var previous = settings.Keys.ToDictionary(
                name => name,
                Environment.GetEnvironmentVariable);
            try
            {
                foreach (var (name, value) in settings)
                    Environment.SetEnvironmentVariable(name, value);

                return CreateClient();
            }
            finally
            {
                foreach (var (name, value) in previous)
                    Environment.SetEnvironmentVariable(name, value);
            }
        }
    }
}
