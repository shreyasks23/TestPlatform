namespace TestPlatform.Domain.Entities;

public sealed class UserAnswer
{
    public Guid QuestionId { get; private set; }
    public Guid SelectedOptionId { get; private set; }

    private UserAnswer() { }

    public UserAnswer(Guid questionId, Guid selectedOptionId)
    {
        QuestionId = questionId;
        SelectedOptionId = selectedOptionId;
    }
}
