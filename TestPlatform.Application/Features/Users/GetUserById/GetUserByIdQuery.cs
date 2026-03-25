using MediatR;
using TestPlatform.Application.Features.Users;

namespace TestPlatform.Application.Features.Users.GetUserById;

public sealed record GetUserByIdQuery(Guid UserId) : IRequest<UserDetailsDto>;
