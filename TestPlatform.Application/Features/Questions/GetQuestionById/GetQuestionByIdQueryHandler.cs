using MediatR;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Application.Features.Questions;
using TestPlatform.Domain.Aggregates;

namespace TestPlatform.Application.Features.Questions.GetQuestionById;

public sealed class GetQuestionByIdQueryHandler(IQuestionRepository questionRepository)
    : IRequestHandler<GetQuestionByIdQuery, QuestionDetailsDto>
{
    public async Task<QuestionDetailsDto> Handle(GetQuestionByIdQuery request, CancellationToken cancellationToken)
    {
        var question = await questionRepository.GetByIdAsync(request.QuestionId, cancellationToken);
        if (question is null)
            throw new EntityNotFoundException(nameof(Question), request.QuestionId);

        return QuestionMappings.ToDetailsDto(question);
    }
}
