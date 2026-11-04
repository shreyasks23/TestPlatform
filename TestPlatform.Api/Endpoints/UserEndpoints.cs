using FluentValidation;
using MediatR;
using TestPlatform.Api.Contracts.Users;
using TestPlatform.Api.Extensions;
using TestPlatform.Application.Common.Exceptions;
using TestPlatform.Application.Common.Security;
using TestPlatform.Application.Features.Users.CreateUser;
using TestPlatform.Application.Features.Users.GetUserById;
using TestPlatform.Application.Features.Users.GetUsers;
using TestPlatform.Application.Features.Users.UpdateUser;

namespace TestPlatform.Api.Endpoints;

internal static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users", CreateUserAsync)
            .WithName("CreateUser")
            .WithTags("Users")
            .RequireAuthorization(AuthConstants.Policies.CanWriteApi);

        app.MapGet("/api/users", GetUsersAsync)
            .WithName("GetUsers")
            .WithTags("Users");

        app.MapGet("/api/users/{id:guid}", GetUserByIdAsync)
            .WithName("GetUserById")
            .WithTags("Users");

        app.MapPut("/api/users/{id:guid}", UpdateUserAsync)
            .WithName("UpdateUser")
            .WithTags("Users")
            .RequireAuthorization(AuthConstants.Policies.CanWriteApi);

        return app;
    }

    private static async Task<IResult> CreateUserAsync(
        CreateUserRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(
                new CreateUserCommand(request.Email, request.FirstName, request.LastName, request.Password),
                cancellationToken);

            return Results.Created($"/api/users/{result.Id}", result);
        }
        catch (ValidationException exception)
        {
            return exception.ToValidationProblem();
        }
        catch (InvalidOperationException exception)
        {
            return exception.ToBadRequestProblem("Invalid user");
        }
    }

    private static async Task<IResult> GetUsersAsync(ISender sender, CancellationToken cancellationToken)
        => Results.Ok(await sender.Send(new GetUsersQuery(), cancellationToken));

    private static async Task<IResult> GetUserByIdAsync(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new GetUserByIdQuery(id), cancellationToken));
        }
        catch (EntityNotFoundException exception)
        {
            return exception.ToNotFoundProblem();
        }
    }

    private static async Task<IResult> UpdateUserAsync(
        Guid id,
        UpdateUserRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(
                new UpdateUserCommand(id, request.Email, request.FirstName, request.LastName),
                cancellationToken));
        }
        catch (ValidationException exception)
        {
            return exception.ToValidationProblem();
        }
        catch (EntityNotFoundException exception)
        {
            return exception.ToNotFoundProblem();
        }
        catch (InvalidOperationException exception)
        {
            return exception.ToBadRequestProblem("Invalid user");
        }
    }
}
