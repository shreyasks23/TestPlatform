using TestPlatform.Domain.Aggregates;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Persistence;

internal sealed class InMemoryStore
{
    private readonly List<Question> _questions = [];
    private readonly List<Test> _tests = [];

    public IReadOnlyList<Question> Questions => _questions.AsReadOnly();
    public IReadOnlyList<Test> Tests => _tests.AsReadOnly();

    public InMemoryStore()
    {
        var firstQuestionOptions = new[]
        {
            new Option("interface"),
            new Option("class"),
            new Option("record"),
            new Option("struct")
        };
        var firstQuestion = new Question(
            "Which C# type is reference-only and supports inheritance by default?",
            firstQuestionOptions,
            firstQuestionOptions[1].Id,
            "Classes are reference types and can be inherited unless they are marked sealed.");

        var secondQuestionOptions = new[]
        {
            new Option("private"),
            new Option("readonly"),
            new Option("sealed"),
            new Option("static")
        };
        var secondQuestion = new Question(
            "Which keyword prevents a class from being inherited?",
            secondQuestionOptions,
            secondQuestionOptions[2].Id,
            "The sealed keyword closes a class to inheritance.");

        _questions.Add(firstQuestion);
        _questions.Add(secondQuestion);

        var csharpBasics = new Test("C# Fundamentals");
        csharpBasics.AddQuestion(firstQuestion.Id);
        csharpBasics.AddQuestion(secondQuestion.Id);

        _tests.Add(csharpBasics);
    }

    public void AddQuestion(Question question)
    {
        ArgumentNullException.ThrowIfNull(question);
        _questions.Add(question);
    }

    public void AddTest(Test test)
    {
        ArgumentNullException.ThrowIfNull(test);
        _tests.Add(test);
    }
}
