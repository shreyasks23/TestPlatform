namespace TestPlatform.Application.Features.Users;

public sealed record UserDetailsDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName);
