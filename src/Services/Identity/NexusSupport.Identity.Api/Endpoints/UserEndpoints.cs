using Microsoft.AspNetCore.Http.HttpResults;
using NexusSupport.Identity.Api.Constants;
using NexusSupport.Identity.Application.Dtos;
using NexusSupport.Identity.Application.Interfaces;

namespace NexusSupport.Identity.Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.Users.Group)
            .WithTags(OpenApiMetadata.Tags.Users);

        group.MapGet("/", GetAllAsync)
            .WithName(OpenApiMetadata.Users.GetAllName)
            .WithSummary(OpenApiMetadata.Users.GetAllSummary)
            .WithDescription(OpenApiMetadata.Users.GetAllDescription);

        group.MapGet(ApiRoutes.Users.ById, GetByIdAsync)
            .WithName(OpenApiMetadata.Users.GetByIdName)
            .WithSummary(OpenApiMetadata.Users.GetByIdSummary)
            .WithDescription(OpenApiMetadata.Users.GetByIdDescription);

        group.MapGet(ApiRoutes.Users.ByExternalSubject, GetByExternalSubjectAsync)
            .WithName(OpenApiMetadata.Users.GetByExternalSubjectName)
            .WithSummary(OpenApiMetadata.Users.GetByExternalSubjectSummary)
            .WithDescription(OpenApiMetadata.Users.GetByExternalSubjectDescription);

        group.MapPost("/", CreateAsync)
            .WithName(OpenApiMetadata.Users.CreateName)
            .WithSummary(OpenApiMetadata.Users.CreateSummary)
            .WithDescription(OpenApiMetadata.Users.CreateDescription);

        group.MapPut("/", UpdateAsync)
            .WithName(OpenApiMetadata.Users.UpdateName)
            .WithSummary(OpenApiMetadata.Users.UpdateSummary)
            .WithDescription(OpenApiMetadata.Users.UpdateDescription);

        group.MapDelete(ApiRoutes.Users.ById, DeleteAsync)
            .WithName(OpenApiMetadata.Users.DeleteName)
            .WithSummary(OpenApiMetadata.Users.DeleteSummary)
            .WithDescription(OpenApiMetadata.Users.DeleteDescription);

        return app;
    }

    private static async Task<Ok<IReadOnlyList<UserDto>>> GetAllAsync(
        IUserService userService, CancellationToken cancellationToken)
        => TypedResults.Ok(await userService.GetAllAsync(cancellationToken));

    private static async Task<Results<Ok<UserDto>, NotFound>> GetByIdAsync(
        Guid id, IUserService userService, CancellationToken cancellationToken)
    {
        var user = await userService.GetByIdAsync(id, cancellationToken);
        return user is null ? TypedResults.NotFound() : TypedResults.Ok(user);
    }

    private static async Task<Results<Ok<UserDto>, NotFound>> GetByExternalSubjectAsync(
        string issuer, string externalSubjectId, IUserService userService, CancellationToken cancellationToken)
    {
        var user = await userService.GetByExternalSubjectAsync(issuer, externalSubjectId, cancellationToken);
        return user is null ? TypedResults.NotFound() : TypedResults.Ok(user);
    }

    private static async Task<Created<UserDto>> CreateAsync(
        UserDto user, IUserService userService, CancellationToken cancellationToken)
    {
        var created = await userService.CreateAsync(user, cancellationToken);
        return TypedResults.Created($"{ApiRoutes.Users.Group}/{created.Id}", created);
    }

    private static async Task<NoContent> UpdateAsync(
        UserDto user, IUserService userService, CancellationToken cancellationToken)
    {
        await userService.UpdateAsync(user, cancellationToken);
        return TypedResults.NoContent();
    }

    private static async Task<NoContent> DeleteAsync(
        Guid id, IUserService userService, CancellationToken cancellationToken)
    {
        await userService.DeleteAsync(id, cancellationToken);
        return TypedResults.NoContent();
    }
}
