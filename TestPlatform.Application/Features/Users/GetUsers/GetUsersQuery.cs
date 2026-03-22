using MediatR;
using TestPlatform.Application.Features.Users;

namespace TestPlatform.Application.Features.Users.GetUsers;

public sealed record GetUsersQuery : IRequest<IReadOnlyList<UserDetailsDto>>;
