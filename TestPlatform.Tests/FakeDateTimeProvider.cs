using TestPlatform.Application.Common.Interfaces.Security;

namespace TestPlatform.Tests;

internal sealed class FakeDateTimeProvider(DateTimeOffset utcNow) : IDateTimeProvider
{
    public DateTimeOffset UtcNow { get; set; } = utcNow;
}
