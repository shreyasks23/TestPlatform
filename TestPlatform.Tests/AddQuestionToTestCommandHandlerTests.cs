using FluentValidation;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Features.Tests.AddQuestionToTest;
using TestPlatform.Domain.Aggregates;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Tests;

public sealed class AddQuestionToTestCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCommandIsValid_AttachesExistingQuestion()
    {
        var test = new Test("C# Basics");
        var question = BuildQuestion();
        var testRepository = new TestRepositoryStub([test]);
        var questionRepository = new QuestionRepositoryStub([question]);
        var handler = new AddQuestionToTestCommandHandler(
            testRepository,
            questionRepository,
            new AddQuestionToTestCommandValidator());

        var result = await handler.Handle(new AddQuestionToTestCommand(test.Id, question.Id), CancellationToken.None);

        Assert.Equal(test.Id, result.TestId);
        Assert.Equal(question.Id, result.QuestionId);
        Assert.Equal(question.ExplanationText, result.ExplanationText);
        Assert.Single(test.QuestionIds);
        Assert.Equal(question.Id, test.QuestionIds[0]);
    }

    [Fact]
    public async Task Handle_WhenQuestionDoesNotExist_ThrowsNotFound()
    {
        var test = new Test("C# Basics");
        var handler = new AddQuestionToTestCommandHandler(
            new TestRepositoryStub([test]),
            new QuestionRepositoryStub([]),
            new AddQuestionToTestCommandValidator());

        await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            handler.Handle(new AddQuestionToTestCommand(test.Id, Guid.CreateVersion7()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenQuestionAlreadyAttached_ThrowsInvalidOperationException()
    {
        var test = new Test("C# Basics");
        var question = BuildQuestion();
        test.AddQuestion(question.Id);

        var handler = new AddQuestionToTestCommandHandler(
            new TestRepositoryStub([test]),
            new QuestionRepositoryStub([question]),
            new AddQuestionToTestCommandValidator());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new AddQuestionToTestCommand(test.Id, question.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenQuestionIsDeleted_ThrowsInvalidOperationException()
    {
        var test = new Test("C# Basics");
        var question = BuildQuestion();
        question.SoftDelete();

        var handler = new AddQuestionToTestCommandHandler(
            new TestRepositoryStub([test]),
            new QuestionRepositoryStub([question]),
            new AddQuestionToTestCommandValidator());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new AddQuestionToTestCommand(test.Id, question.Id), CancellationToken.None));
    }

    private static Question BuildQuestion()
    {
        var options = new[]
        {
            new Option("class"),
            new Option("struct")
        };

        return new Question(
            "Which type supports inheritance by default?",
            options,
            options[0].Id,
            "Classes support inheritance by default.");
    }
}
