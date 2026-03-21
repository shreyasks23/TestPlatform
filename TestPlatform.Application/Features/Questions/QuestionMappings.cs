using TestPlatform.Domain.Aggregates;

namespace TestPlatform.Application.Features.Questions;

internal static class QuestionMappings
{
    public static QuestionDetailsDto ToDetailsDto(Question question)
        => new(
            question.Id,
            question.Text,
            question.ExplanationText,
            question.CorrectOptionId,
            question.IsDeleted,
            question.Options
                .Select(option => new QuestionOptionDto(option.Id, option.Text))
                .ToArray());
}
