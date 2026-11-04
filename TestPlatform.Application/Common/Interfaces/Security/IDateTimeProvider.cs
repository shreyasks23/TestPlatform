namespace TestPlatform.Application.Common.Interfaces.Security;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
