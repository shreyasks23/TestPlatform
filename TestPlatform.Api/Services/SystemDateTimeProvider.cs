using TestPlatform.Application.Common.Interfaces.Security;

namespace TestPlatform.Api.Services;

internal sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
