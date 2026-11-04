using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TestPlatform.Application.Common.Exceptions;

namespace TestPlatform.Api.Extensions;

internal static class ProblemDetailsExtensions
{
    public static IResult ToValidationProblem(this ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).Distinct().ToArray());

        return Results.ValidationProblem(errors);
    }

    public static IResult ToNotFoundProblem(this EntityNotFoundException exception)
    {
        return Results.NotFound(new ProblemDetails
        {
            Title = "Resource not found",
            Detail = exception.Message,
            Status = StatusCodes.Status404NotFound
        });
    }

    public static IResult ToBadRequestProblem(this InvalidOperationException exception, string title)
    {
        return Results.BadRequest(new ProblemDetails
        {
            Title = title,
            Detail = exception.Message,
            Status = StatusCodes.Status400BadRequest
        });
    }

    public static IResult ToUnauthorizedProblem(this AuthenticationFailedException exception, string title)
    {
        return Results.Problem(new ProblemDetails
        {
            Title = title,
            Detail = exception.Message,
            Status = StatusCodes.Status401Unauthorized
        });
    }
}
