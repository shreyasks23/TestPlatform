using FluentValidation;

namespace TestPlatform.Application.Features.TestSubmissions.SubmitTest;

public sealed class SubmitTestCommandValidator : AbstractValidator<SubmitTestCommand>
{
    public SubmitTestCommandValidator()
    {
        RuleFor(command => command.TestId)
            .NotEmpty();

        RuleFor(command => command.UserId)
            .NotEmpty();

        RuleFor(command => command.Answers)
            .NotEmpty();

        RuleForEach(command => command.Answers)
            .ChildRules(answer =>
            {
                answer.RuleFor(item => item.QuestionId)
                    .NotEmpty();

                answer.RuleFor(item => item.SelectedOptionId)
                    .NotEmpty();
            });

        RuleFor(command => command.Answers)
            .Must(answers => answers.Select(answer => answer.QuestionId).Distinct().Count() == answers.Count)
            .WithMessage("Each question can only be answered once");
    }
}
