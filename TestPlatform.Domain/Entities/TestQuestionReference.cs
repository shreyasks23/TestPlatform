namespace TestPlatform.Domain.Entities;

public sealed class TestQuestionReference
{
    public Guid QuestionId { get; private set; }
    public int Order { get; private set; }

    private TestQuestionReference() { }

    public TestQuestionReference(Guid questionId, int order)
    {
        if (questionId == Guid.Empty)
            throw new ArgumentException("Question ID cannot be empty", nameof(questionId));

        if (order < 0)
            throw new ArgumentOutOfRangeException(nameof(order), "Order must be non-negative");

        QuestionId = questionId;
        Order = order;
    }
}
