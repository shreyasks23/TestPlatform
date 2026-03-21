namespace TestPlatform.Api.Contracts.Tests;

public sealed record SubmitTestRequest(
    Guid UserId,
    IReadOnlyCollection<SubmitTestAnswerRequest> Answers);

public sealed record SubmitTestAnswerRequest(
    Guid QuestionId,
    Guid SelectedOptionId);
