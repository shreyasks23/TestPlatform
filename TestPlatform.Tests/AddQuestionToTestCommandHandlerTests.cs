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
        var actorId = Guid.CreateVersion7();
        var test = new Test("C# Basics", actorId);
        var question = BuildQuestion(actorId);
        var handler = CreateHandler(test, question);

        var result = await handler.Handle(new AddQuestionToTestCommand(test.Id, question.Id, actorId, false), CancellationToken.None);

        Assert.Equal(test.Id, result.TestId);
        Assert.Equal(question.Id, result.QuestionId);
        Assert.Single(test.QuestionIds);
    }

    [Fact]
    public async Task Handle_WhenActorDoesNotOwnTest_ThrowsForbidden()
    {
        var test = new Test("C# Basics", Guid.CreateVersion7());
        var question = BuildQuestion(Guid.CreateVersion7());
        var handler = CreateHandler(test, question);

        await Assert.ThrowsAsync<AuthorizationDeniedException>(() =>
            handler.Handle(new AddQuestionToTestCommand(test.Id, question.Id, Guid.CreateVersion7(), false), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenQuestionIsDeleted_ThrowsInvalidOperationException()
    {
        var actorId = Guid.CreateVersion7();
        var test = new Test("C# Basics", actorId);
        var question = BuildQuestion(actorId);
        question.SoftDelete();
        var handler = CreateHandler(test, question);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new AddQuestionToTestCommand(test.Id, question.Id, actorId, false), CancellationToken.None));
    }

    private static AddQuestionToTestCommandHandler CreateHandler(Test test, Question question) => new(
        new TestRepositoryStub([test]),
        new QuestionRepositoryStub([question]),
        new AddQuestionToTestCommandValidator());

    private static Question BuildQuestion(Guid ownerId)
    {
        var options = new[] { new Option("class"), new Option("struct") };
        return new Question("Which type supports inheritance by default?", options, options[0].Id, "Classes support inheritance by default.", ownerId);
    }
}
