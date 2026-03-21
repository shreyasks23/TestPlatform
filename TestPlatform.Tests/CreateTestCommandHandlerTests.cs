using FluentValidation;
using TestPlatform.Application.Features.Tests.CreateTest;

namespace TestPlatform.Tests;

public sealed class CreateTestCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenCommandIsValid_CreatesAndReturnsTest()
    {
        var repository = new TestRepositoryStub([]);
        var handler = new CreateTestCommandHandler(repository, new CreateTestCommandValidator());

        var command = new CreateTestCommand("Algorithms");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal("Algorithms", result.Title);
        Assert.Equal(0, result.QuestionCount);
        Assert.NotNull(repository.AddedTest);
        Assert.Equal(result.Id, repository.AddedTest!.Id);
    }

    [Fact]
    public async Task Handle_WhenTitleIsEmpty_ThrowsValidationException()
    {
        var repository = new TestRepositoryStub([]);
        var handler = new CreateTestCommandHandler(repository, new CreateTestCommandValidator());

        var command = new CreateTestCommand(string.Empty);

        await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, CancellationToken.None));
        Assert.Null(repository.AddedTest);
    }
}
