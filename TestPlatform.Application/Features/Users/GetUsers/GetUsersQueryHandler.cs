using MediatR;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Application.Features.Users;

namespace TestPlatform.Application.Features.Users.GetUsers;

public sealed class GetUsersQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDetailsDto>>
{
    public async Task<IReadOnlyList<UserDetailsDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);
        return users.Select(UserMappings.ToDetailsDto).ToArray();
    }
}
