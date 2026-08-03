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
            .WithName(OpenApiMetadata.Roles.GetAllName)
            .WithSummary(OpenApiMetadata.Roles.GetAllSummary)
            .WithDescription(OpenApiMetadata.Roles.GetAllDescription);

        group.MapGet(ApiRoutes.Roles.ById, GetByIdAsync)
            .WithName(OpenApiMetadata.Roles.GetByIdName)
            .WithSummary(OpenApiMetadata.Roles.GetByIdSummary)
            .WithDescription(OpenApiMetadata.Roles.GetByIdDescription);

        group.MapGet(ApiRoutes.Roles.ByCode, GetByCodeAsync)
            .WithName(OpenApiMetadata.Roles.GetByCodeName)
            .WithSummary(OpenApiMetadata.Roles.GetByCodeSummary)
            .WithDescription(OpenApiMetadata.Roles.GetByCodeDescription);

        group.MapPost("/", CreateAsync)
            .WithName(OpenApiMetadata.Roles.CreateName)
            .WithSummary(OpenApiMetadata.Roles.CreateSummary)
            .WithDescription(OpenApiMetadata.Roles.CreateDescription);

        group.MapPut("/", UpdateAsync)
            .WithName(OpenApiMetadata.Roles.UpdateName)
            .WithSummary(OpenApiMetadata.Roles.UpdateSummary)
            .WithDescription(OpenApiMetadata.Roles.UpdateDescription);

        group.MapDelete(ApiRoutes.Roles.ById, DeleteAsync)
            .WithName(OpenApiMetadata.Roles.DeleteName)
            .WithSummary(OpenApiMetadata.Roles.DeleteSummary)
            .WithDescription(OpenApiMetadata.Roles.DeleteDescription);

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
        RolDto rol, IRolService rolService, CancellationToken cancellationToken)
    {
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
