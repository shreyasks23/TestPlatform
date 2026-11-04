using MediatR;
using TestPlatform.Application.Features.Users;

namespace TestPlatform.Application.Features.Users.CreateUser;

public sealed record CreateUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string Password) : IRequest<UserDetailsDto>;
