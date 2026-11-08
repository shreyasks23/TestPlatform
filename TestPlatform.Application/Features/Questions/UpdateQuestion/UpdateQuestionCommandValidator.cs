using FluentValidation;
using TestPlatform.Application.Features.Questions;

namespace TestPlatform.Application.Features.Questions.UpdateQuestion;

public sealed class UpdateQuestionCommandValidator : AbstractValidator<UpdateQuestionCommand>
{
    public UpdateQuestionCommandValidator()
    {
        RuleFor(command => command.QuestionId)
            .NotEmpty();

        RuleFor(command => command.Text)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(command => command.ExplanationText)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(command => command.ActorId).NotEmpty();

        RuleFor(command => command.Options)
            .NotNull()
            .Must(options => options.Count >= 2)
            .WithMessage("A question must have at least 2 options");

        RuleForEach(command => command.Options)
            .ChildRules(option =>
            {
                option.RuleFor(item => item.Text)
                    .NotEmpty()
                    .MaximumLength(200);
            });

        RuleFor(command => command.CorrectOptionIndex)
            .GreaterThanOrEqualTo(0);

        RuleFor(command => command)
            .Must(command => command.Options is not null && command.CorrectOptionIndex < command.Options.Count)
            .WithMessage("Correct option index is out of range");
    }
}
