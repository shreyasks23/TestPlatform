using FluentValidation;
using MediatR;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Application.Features.Questions;
using TestPlatform.Domain.Aggregates;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Application.Features.Questions.CreateQuestion;

public sealed class CreateQuestionCommandHandler(
    IQuestionRepository questionRepository,
    IValidator<CreateQuestionCommand> validator) : IRequestHandler<CreateQuestionCommand, QuestionDetailsDto>
{
    public async Task<QuestionDetailsDto> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var options = request.Options
            .Select(option => new Option(option.Text))
            .ToArray();

        var question = new Question(
            request.Text,
            options,
            options[request.CorrectOptionIndex].Id,
            request.ExplanationText,
            request.ActorId);

        await questionRepository.AddAsync(question, cancellationToken);

        return QuestionMappings.ToDetailsDto(question);
    }
}
