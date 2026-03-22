using FluentValidation;
using MediatR;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Application.Features.Questions;
using TestPlatform.Domain.Aggregates;
using TestPlatform.Domain.Entities;

namespace TestPlatform.Application.Features.Questions.UpdateQuestion;

public sealed class UpdateQuestionCommandHandler(
    IQuestionRepository questionRepository,
    IValidator<UpdateQuestionCommand> validator) : IRequestHandler<UpdateQuestionCommand, QuestionDetailsDto>
{
    public async Task<QuestionDetailsDto> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var question = await questionRepository.GetByIdAsync(request.QuestionId, cancellationToken);
        if (question is null)
            throw new EntityNotFoundException(nameof(Question), request.QuestionId);

        var options = request.Options
            .Select(option => option.Id.HasValue
                ? new Option(option.Id.Value, option.Text)
                : new Option(option.Text))
            .ToArray();

        question.Update(
            request.Text,
            options,
            options[request.CorrectOptionIndex].Id,
            request.ExplanationText);

        await questionRepository.UpdateAsync(question, cancellationToken);

        return QuestionMappings.ToDetailsDto(question);
    }
}
