using FluentValidation;
using TestPlatform.Application.Features.Questions.CreateQuestion;

namespace TestPlatform.Tests;

public sealed class CreateQuestionCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCommandIsValid_CreatesQuestionWithExplanation()
    {
        var repository = new QuestionRepositoryStub([]);
        var handler = new CreateQuestionCommandHandler(repository, new CreateQuestionCommandValidator());

        var result = await handler.Handle(
            new CreateQuestionCommand(
                "What does sealed do?",
                [new(null, "Allows inheritance"), new(null, "Prevents inheritance")],
                1,
                "sealed prevents a class from being inherited.",
                Guid.CreateVersion7()),
            CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("sealed prevents a class from being inherited.", result.ExplanationText);
        Assert.Equal(result.CorrectOptionId, repository.AddedQuestion!.CorrectOptionId);
    }

    [Fact]
    public async Task Handle_WhenExplanationIsMissing_ThrowsValidationException()
    {
        var repository = new QuestionRepositoryStub([]);
        var handler = new CreateQuestionCommandHandler(repository, new CreateQuestionCommandValidator());

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(
                new CreateQuestionCommand(
                    "Question",
                    [new(null, "A"), new(null, "B")],
                    0,
                    string.Empty,
                    Guid.CreateVersion7()),
                CancellationToken.None));
    }
}
