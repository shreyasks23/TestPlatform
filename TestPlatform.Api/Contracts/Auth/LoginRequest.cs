namespace TestPlatform.Api.Contracts.Auth;

public sealed record LoginRequest(
    string Email,
    string Password);
