using TestPlatform.Domain.Entities;

namespace TestPlatform.Domain.Aggregates;

public sealed class TestAttempt
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid TestId { get; private set; }

    private readonly List<UserAnswer> _answers = [];
    public IReadOnlyList<UserAnswer> Answers => _answers.AsReadOnly();

    public int Score { get; private set; }
    public bool IsEvaluated { get; private set; }

    private TestAttempt() { }

    public TestAttempt(Guid userId, Guid testId)
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        TestId = testId;
    }

    public void AddAnswer(Guid questionId, Guid selectedOptionId)
    {
        if (_answers.Any(a => a.QuestionId == questionId))
            throw new InvalidOperationException("Question already answered");

        _answers.Add(new UserAnswer(questionId, selectedOptionId));
    }

    public void Evaluate(IEnumerable<Question> questions)
    {
        ArgumentNullException.ThrowIfNull(questions);

        if (IsEvaluated)
            throw new InvalidOperationException("Already evaluated");

        var questionLookup = questions.ToDictionary(question => question.Id);

        if (_answers.Count != questionLookup.Count)
            throw new InvalidOperationException("All test questions must be answered before evaluation");

        int score = 0;

        foreach (var answer in _answers)
        {
            if (!questionLookup.TryGetValue(answer.QuestionId, out var question))
                throw new InvalidOperationException($"Question '{answer.QuestionId}' was not found for evaluation");

            if (!question.Options.Any(option => option.Id == answer.SelectedOptionId))
                throw new InvalidOperationException(
                    $"Selected option '{answer.SelectedOptionId}' does not belong to question '{answer.QuestionId}'");

            if (question.CorrectOptionId == answer.SelectedOptionId)
                score++;
        }

        Score = score;
        IsEvaluated = true;
    }
}
