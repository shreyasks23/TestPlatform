namespace TestPlatform.Api.Contracts.Users;

public sealed record UpdateUserRequest(
    string Email,
    string FirstName,
    string LastName,
    string Role);
