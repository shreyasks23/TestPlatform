using FluentValidation;

namespace TestPlatform.Application.Features.Tests.CreateTest;

public sealed class CreateTestCommandValidator : AbstractValidator<CreateTestCommand>
{
    public CreateTestCommandValidator()
    {
        RuleFor(command => command.Title)
            .NotEmpty()
            .MaximumLength(200);
    }
}
