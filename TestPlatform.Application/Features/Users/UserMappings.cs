using TestPlatform.Domain.Entities;

namespace TestPlatform.Application.Features.Users;

internal static class UserMappings
{
    public static UserDetailsDto ToDetailsDto(User user)
        => new(user.Id, user.Email, user.FirstName, user.LastName);
}
