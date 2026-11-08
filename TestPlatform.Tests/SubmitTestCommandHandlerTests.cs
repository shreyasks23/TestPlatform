using FluentValidation;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Application.Common.Security;
using TestPlatform.Application.Features.TestSubmissions.SubmitTest;
using TestPlatform.Domain.Aggregates;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Tests;

public sealed class SubmitTestCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenAnswersAreCorrect_ReturnsEvaluatedResultWithExplanations()
    {
        var (test, questions) = BuildTestAndQuestions();
        var userId = Guid.Parse("00000000-0000-0000-0000-920000000001");
        var userRepository = new UserRepositoryStub([new User(userId, "ada@example.com", "Ada", "Lovelace", "hashed::pw", AuthConstants.Roles.Admin)]);
        var handler = new SubmitTestCommandHandler(
            new TestRepositoryStub([test]),
            new QuestionRepositoryStub(questions),
            userRepository,
            new FakeTestAttemptRepository(),
            new SubmitTestCommandValidator());

        var command = new SubmitTestCommand(
            test.Id,
            userId,
            questions
                .Select(question => new SubmitTestAnswerDto(question.Id, question.CorrectOptionId))
                .ToArray());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(questions.Count, result.Score);
        Assert.Equal(questions.Count, result.TotalQuestions);
        Assert.All(result.Questions, review => Assert.False(string.IsNullOrWhiteSpace(review.ExplanationText)));
    }

    [Fact]
    public async Task Handle_WhenTestDoesNotExist_ThrowsNotFound()
    {
        var handler = new SubmitTestCommandHandler(
            new TestRepositoryStub([]),
            new QuestionRepositoryStub([]),
            new UserRepositoryStub([]),
            new FakeTestAttemptRepository(),
            new SubmitTestCommandValidator());

        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            handler.Handle(
                new SubmitTestCommand(
                    Guid.CreateVersion7(),
                    Guid.CreateVersion7(),
                    [new SubmitTestAnswerDto(Guid.CreateVersion7(), Guid.CreateVersion7())]),
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenSameQuestionIsSubmittedTwice_ThrowsValidationException()
    {
        var (test, questions) = BuildTestAndQuestions();
        var question = questions[0];
        var userId = Guid.Parse("00000000-0000-0000-0000-920000000002");
        var userRepository = new UserRepositoryStub([new User(userId, "grace@example.com", "Grace", "Hopper", "hashed::pw", AuthConstants.Roles.Admin)]);
        var handler = new SubmitTestCommandHandler(
            new TestRepositoryStub([test]),
            new QuestionRepositoryStub(questions),
            userRepository,
            new FakeTestAttemptRepository(),
            new SubmitTestCommandValidator());

        var command = new SubmitTestCommand(
            test.Id,
            userId,
            [
                new SubmitTestAnswerDto(question.Id, question.Options[0].Id),
                new SubmitTestAnswerDto(question.Id, question.Options[1].Id)
            ]);

        await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenNotAllQuestionsAreAnswered_ThrowsInvalidOperationException()
    {
        var (test, questions) = BuildTestAndQuestions();
        var firstQuestion = questions[0];
        var userId = Guid.Parse("00000000-0000-0000-0000-920000000003");
        var userRepository = new UserRepositoryStub([new User(userId, "alan@example.com", "Alan", "Turing", "hashed::pw", AuthConstants.Roles.Admin)]);
        var handler = new SubmitTestCommandHandler(
            new TestRepositoryStub([test]),
            new QuestionRepositoryStub(questions),
            userRepository,
            new FakeTestAttemptRepository(),
            new SubmitTestCommandValidator());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(
                new SubmitTestCommand(
                    test.Id,
                    userId,
                    [new SubmitTestAnswerDto(firstQuestion.Id, firstQuestion.CorrectOptionId)]),
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenSelectedOptionDoesNotBelongToQuestion_ThrowsInvalidOperationException()
    {
        var (test, questions) = BuildTestAndQuestions();
        var firstQuestion = questions[0];
        var secondQuestion = questions[1];
        var userId = Guid.Parse("00000000-0000-0000-0000-920000000004");
        var userRepository = new UserRepositoryStub([new User(userId, "donald@example.com", "Donald", "Knuth", "hashed::pw", AuthConstants.Roles.Admin)]);
        var handler = new SubmitTestCommandHandler(
            new TestRepositoryStub([test]),
            new QuestionRepositoryStub(questions),
            userRepository,
            new FakeTestAttemptRepository(),
            new SubmitTestCommandValidator());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(
                new SubmitTestCommand(
                    test.Id,
                    userId,
                    [
                        new SubmitTestAnswerDto(firstQuestion.Id, secondQuestion.Options[0].Id),
                        new SubmitTestAnswerDto(secondQuestion.Id, secondQuestion.CorrectOptionId)
                    ]),
                CancellationToken.None));
    }

    private static (Test Test, IReadOnlyList<Question> Questions) BuildTestAndQuestions()
    {
        var firstOptions = new[]
        {
            new Option("A"),
            new Option("B")
        };
        var contentOwnerId = Guid.Parse("00000000-0000-0000-0000-920000000010");
        var firstQuestion = new Question("Question 1", firstOptions, firstOptions[0].Id, "A is correct.", contentOwnerId);

        var secondOptions = new[]
        {
            new Option("True"),
            new Option("False")
        };
        var secondQuestion = new Question("Question 2", secondOptions, secondOptions[1].Id, "False is correct.", contentOwnerId);

        var test = new Test("Sample Test", contentOwnerId);
        test.AddQuestion(firstQuestion.Id);
        test.AddQuestion(secondQuestion.Id);

        return (test, [firstQuestion, secondQuestion]);
    }

    private sealed class FakeTestAttemptRepository : ITestAttemptRepository
    {
        public TestAttempt? SavedAttempt { get; private set; }

        public Task AddAsync(TestAttempt attempt, CancellationToken cancellationToken)
        {
            SavedAttempt = attempt;
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<TestAttempt>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<TestAttempt>>([]);
    }
}
