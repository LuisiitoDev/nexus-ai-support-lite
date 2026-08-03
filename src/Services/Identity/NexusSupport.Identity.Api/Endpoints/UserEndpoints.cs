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
            .WithName("GetUsers")
            .WithSummary("Lists users")
            .WithDescription("Returns every locally provisioned user across all tenants.");

        group.MapGet(ApiRoutes.Users.ById, GetByIdAsync)
            .WithName("GetUserById")
            .WithSummary("Gets a user by id")
            .WithDescription("Returns a single user by its Nexus-local identifier.");

        group.MapGet(ApiRoutes.Users.ByExternalSubject, GetByExternalSubjectAsync)
            .WithName("GetUserByExternalSubject")
            .WithSummary("Gets a user by external subject")
            .WithDescription("Resolves the local user provisioned for a validated Entra issuer and subject identifier, per ADR-002.");

        group.MapPost("/", CreateAsync)
            .WithName("CreateUser")
            .WithSummary("Creates a user")
            .WithDescription("Provisions a new local user record.");

        group.MapPut(ApiRoutes.Users.ById, UpdateAsync)
            .WithName("UpdateUser")
            .WithSummary("Updates a user")
            .WithDescription("Updates an existing user's profile, status, or last login.");

        group.MapDelete(ApiRoutes.Users.ById, DeleteAsync)
            .WithName("DeleteUser")
            .WithSummary("Deletes a user")
            .WithDescription("Permanently removes a local user record.");

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
        Guid id, UserDto user, IUserService userService, CancellationToken cancellationToken)
    {
        user.Id = id;
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
