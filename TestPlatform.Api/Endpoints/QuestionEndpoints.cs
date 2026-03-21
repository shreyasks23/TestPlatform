using FluentValidation;
using MediatR;
using TestPlatform.Api.Contracts.Questions;
using TestPlatform.Api.Extensions;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Features.Questions;
using TestPlatform.Application.Features.Questions.CreateQuestion;
using TestPlatform.Application.Features.Questions.DeleteQuestion;
using TestPlatform.Application.Features.Questions.GetQuestionById;
using TestPlatform.Application.Features.Questions.GetQuestions;
using TestPlatform.Application.Features.Questions.UpdateQuestion;

namespace TestPlatform.Api.Endpoints;

internal static class QuestionEndpoints
{
    public static IEndpointRouteBuilder MapQuestionEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/questions", CreateQuestionAsync)
            .WithName("CreateQuestion")
            .WithTags("Questions");

        app.MapGet("/api/questions", GetQuestionsAsync)
            .WithName("GetQuestions")
            .WithTags("Questions");

        app.MapGet("/api/questions/{id:guid}", GetQuestionByIdAsync)
            .WithName("GetQuestionById")
            .WithTags("Questions");

        app.MapPut("/api/questions/{id:guid}", UpdateQuestionAsync)
            .WithName("UpdateQuestion")
            .WithTags("Questions");

        app.MapDelete("/api/questions/{id:guid}", DeleteQuestionAsync)
            .WithName("DeleteQuestion")
            .WithTags("Questions");

        return app;
    }

    private static async Task<IResult> CreateQuestionAsync(
        CreateQuestionRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(
                new CreateQuestionCommand(
                    request.Text,
                    request.Options.Select(option => new QuestionOptionInputDto(option.Text)).ToArray(),
                    request.CorrectOptionIndex,
                    request.ExplanationText),
                cancellationToken);

            return Results.Created($"/api/questions/{result.Id}", result);
        }
        catch (ValidationException exception)
        {
            return exception.ToValidationProblem();
        }
    }

    private static async Task<IResult> GetQuestionsAsync(ISender sender, CancellationToken cancellationToken)
        => Results.Ok(await sender.Send(new GetQuestionsQuery(), cancellationToken));

    private static async Task<IResult> GetQuestionByIdAsync(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new GetQuestionByIdQuery(id), cancellationToken));
        }
        catch (EntityNotFoundException exception)
        {
            return exception.ToNotFoundProblem();
        }
    }

    private static async Task<IResult> UpdateQuestionAsync(
        Guid id,
        UpdateQuestionRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(
                new UpdateQuestionCommand(
                    id,
                    request.Text,
                    request.Options.Select(option => new QuestionOptionInputDto(option.Text)).ToArray(),
                    request.CorrectOptionIndex,
                    request.ExplanationText),
                cancellationToken);

            return Results.Ok(result);
        }
        catch (ValidationException exception)
        {
            return exception.ToValidationProblem();
        }
        catch (EntityNotFoundException exception)
        {
            return exception.ToNotFoundProblem();
        }
    }

    private static async Task<IResult> DeleteQuestionAsync(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            await sender.Send(new DeleteQuestionCommand(id), cancellationToken);
            return Results.NoContent();
        }
        catch (EntityNotFoundException exception)
        {
            return exception.ToNotFoundProblem();
        }
    }
}
