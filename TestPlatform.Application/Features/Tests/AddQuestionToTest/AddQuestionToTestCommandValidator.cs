using FluentValidation;

namespace TestPlatform.Application.Features.Tests.AddQuestionToTest;

public sealed class AddQuestionToTestCommandValidator : AbstractValidator<AddQuestionToTestCommand>
{
    public AddQuestionToTestCommandValidator()
    {
        RuleFor(command => command.TestId)
            .NotEmpty();

        RuleFor(command => command.QuestionId)
            .NotEmpty();

        RuleFor(command => command.ActorId).NotEmpty();
    }
}
