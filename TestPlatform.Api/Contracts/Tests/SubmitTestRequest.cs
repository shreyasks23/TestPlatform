namespace TestPlatform.Api.Contracts.Tests;

public sealed record SubmitTestRequest(IReadOnlyCollection<SubmitTestAnswerRequest> Answers);

public sealed record SubmitTestAnswerRequest(
    Guid QuestionId,
    Guid SelectedOptionId);
