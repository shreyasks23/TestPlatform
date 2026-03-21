namespace TestPlatform.Domain.Events;

public sealed class TestSubmittedEvent
{
    public Guid AttemptId { get; }

    public TestSubmittedEvent(Guid attemptId)
    {
        AttemptId = attemptId;
    }
}
