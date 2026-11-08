using MediatR;

namespace TestPlatform.Application.Features.TestAttempts.GetAttemptReview;

public sealed record GetAttemptReviewQuery(Guid AttemptId, Guid UserId) : IRequest<TestAttemptReviewDto>;
