using TestPlatform.Application.Common.Models;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Application.Common.Interfaces.Security;

public interface IAccessTokenService
{
    AccessTokenResult CreateToken(User user);
}
