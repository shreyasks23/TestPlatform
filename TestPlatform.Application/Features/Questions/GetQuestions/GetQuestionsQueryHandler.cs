using MediatR;
using TestPlatform.Application.Common.Interfaces.Repositories;
using TestPlatform.Application.Features.Questions;

namespace TestPlatform.Application.Features.Questions.GetQuestions;

public sealed class GetQuestionsQueryHandler(IQuestionRepository questionRepository)
    : IRequestHandler<GetQuestionsQuery, IReadOnlyList<QuestionDetailsDto>>
{
    public async Task<IReadOnlyList<QuestionDetailsDto>> Handle(GetQuestionsQuery request, CancellationToken cancellationToken)
    {
        var questions = await questionRepository.GetAllActiveAsync(cancellationToken);
        return questions.Select(QuestionMappings.ToDetailsDto).ToArray();
    }
}
