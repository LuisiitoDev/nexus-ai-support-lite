using Microsoft.AspNetCore.Http.HttpResults;
using NexusSupport.Identity.Api.Constants;
using NexusSupport.Identity.Application.Dtos;
using NexusSupport.Identity.Application.Interfaces;

namespace NexusSupport.Identity.Api.Endpoints;

public static class RolEndpoints
{
    public static IEndpointRouteBuilder MapRolEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.Roles.Group)
            .WithTags(OpenApiMetadata.Tags.Roles);

        group.MapGet("/", GetAllAsync)
            .WithName("GetRoles")
            .WithSummary("Lists product roles")
            .WithDescription("Returns every Nexus-managed product role (e.g. Requester, Agent, Administrator).");

        group.MapGet(ApiRoutes.Roles.ById, GetByIdAsync)
            .WithName("GetRoleById")
            .WithSummary("Gets a product role by id")
            .WithDescription("Returns a single product role by its Nexus-local identifier.");

        group.MapGet(ApiRoutes.Roles.ByCode, GetByCodeAsync)
            .WithName("GetRoleByCode")
            .WithSummary("Gets a product role by code")
            .WithDescription("Returns a single product role by its unique code.");

        group.MapPost("/", CreateAsync)
            .WithName("CreateRole")
            .WithSummary("Creates a product role")
            .WithDescription("Creates a new Nexus-managed product role.");

        group.MapPut(ApiRoutes.Roles.ById, UpdateAsync)
            .WithName("UpdateRole")
            .WithSummary("Updates a product role")
            .WithDescription("Updates an existing product role's name, description, or active status.");

        group.MapDelete(ApiRoutes.Roles.ById, DeleteAsync)
            .WithName("DeleteRole")
            .WithSummary("Deletes a product role")
            .WithDescription("Permanently removes a product role.");

        return app;
    }

    private static async Task<Ok<IReadOnlyList<RolDto>>> GetAllAsync(
        IRolService rolService, CancellationToken cancellationToken)
        => TypedResults.Ok(await rolService.GetAllAsync(cancellationToken));

    private static async Task<Results<Ok<RolDto>, NotFound>> GetByIdAsync(
        int id, IRolService rolService, CancellationToken cancellationToken)
    {
        var rol = await rolService.GetByIdAsync(id, cancellationToken);
        return rol is null ? TypedResults.NotFound() : TypedResults.Ok(rol);
    }

    private static async Task<Results<Ok<RolDto>, NotFound>> GetByCodeAsync(
        string code, IRolService rolService, CancellationToken cancellationToken)
    {
        var rol = await rolService.GetByCodeAsync(code, cancellationToken);
        return rol is null ? TypedResults.NotFound() : TypedResults.Ok(rol);
    }

    private static async Task<Created<RolDto>> CreateAsync(
        RolDto rol, IRolService rolService, CancellationToken cancellationToken)
    {
        var created = await rolService.CreateAsync(rol, cancellationToken);
        return TypedResults.Created($"{ApiRoutes.Roles.Group}/{created.Id}", created);
    }

    private static async Task<NoContent> UpdateAsync(
        int id, RolDto rol, IRolService rolService, CancellationToken cancellationToken)
    {
        rol.Id = id;
        await rolService.UpdateAsync(rol, cancellationToken);
        return TypedResults.NoContent();
    }

    private static async Task<NoContent> DeleteAsync(
        int id, IRolService rolService, CancellationToken cancellationToken)
    {
        await rolService.DeleteAsync(id, cancellationToken);
        return TypedResults.NoContent();
    }
}
