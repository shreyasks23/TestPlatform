namespace TestPlatform.Application.Common.Interfaces.Security;

public interface IAuthTokenLifetimeProvider
{
    TimeSpan RefreshTokenLifetime { get; }
}
