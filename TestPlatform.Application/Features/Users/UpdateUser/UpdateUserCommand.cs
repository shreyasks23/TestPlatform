using MediatR;
using TestPlatform.Application.Features.Users;

namespace TestPlatform.Application.Features.Users.UpdateUser;

public sealed record UpdateUserCommand(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string Role) : IRequest<UserDetailsDto>;
