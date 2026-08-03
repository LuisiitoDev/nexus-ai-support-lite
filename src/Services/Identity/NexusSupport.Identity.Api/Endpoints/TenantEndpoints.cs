using Microsoft.AspNetCore.Http.HttpResults;
using NexusSupport.Identity.Api.Constants;
using NexusSupport.Identity.Application.Dtos;
using NexusSupport.Identity.Application.Interfaces;

namespace NexusSupport.Identity.Api.Endpoints;

public static class TenantEndpoints
{
    public static IEndpointRouteBuilder MapTenantEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.Tenants.Group)
            .WithTags(OpenApiMetadata.Tags.Tenants);

        group.MapGet("/", GetAllAsync)
            .WithName(OpenApiMetadata.Tenants.GetAllName)
            .WithSummary(OpenApiMetadata.Tenants.GetAllSummary)
            .WithDescription(OpenApiMetadata.Tenants.GetAllDescription);

        group.MapGet(ApiRoutes.Tenants.ById, GetByIdAsync)
            .WithName(OpenApiMetadata.Tenants.GetByIdName)
            .WithSummary(OpenApiMetadata.Tenants.GetByIdSummary)
            .WithDescription(OpenApiMetadata.Tenants.GetByIdDescription);

        group.MapGet(ApiRoutes.Tenants.ByEntraTenantId, GetByEntraTenantIdAsync)
            .WithName(OpenApiMetadata.Tenants.GetByEntraTenantIdName)
            .WithSummary(OpenApiMetadata.Tenants.GetByEntraTenantIdSummary)
            .WithDescription(OpenApiMetadata.Tenants.GetByEntraTenantIdDescription);

        group.MapPost("/", CreateAsync)
            .WithName(OpenApiMetadata.Tenants.CreateName)
            .WithSummary(OpenApiMetadata.Tenants.CreateSummary)
            .WithDescription(OpenApiMetadata.Tenants.CreateDescription);

        group.MapPut("/", UpdateAsync)
            .WithName(OpenApiMetadata.Tenants.UpdateName)
            .WithSummary(OpenApiMetadata.Tenants.UpdateSummary)
            .WithDescription(OpenApiMetadata.Tenants.UpdateDescription);

        group.MapDelete(ApiRoutes.Tenants.ById, DeleteAsync)
            .WithName(OpenApiMetadata.Tenants.DeleteName)
            .WithSummary(OpenApiMetadata.Tenants.DeleteSummary)
            .WithDescription(OpenApiMetadata.Tenants.DeleteDescription);

        return app;
    }

    private static async Task<Ok<IReadOnlyList<TenantDto>>> GetAllAsync(
        ITenantService tenantService, CancellationToken cancellationToken)
        => TypedResults.Ok(await tenantService.GetAllAsync(cancellationToken));

    private static async Task<Results<Ok<TenantDto>, NotFound>> GetByIdAsync(
        Guid id, ITenantService tenantService, CancellationToken cancellationToken)
    {
        var tenant = await tenantService.GetByIdAsync(id, cancellationToken);
        return tenant is null ? TypedResults.NotFound() : TypedResults.Ok(tenant);
    }

    private static async Task<Results<Ok<TenantDto>, NotFound>> GetByEntraTenantIdAsync(
        string entraTenantId, ITenantService tenantService, CancellationToken cancellationToken)
    {
        var tenant = await tenantService.GetByEntraTenantIdAsync(entraTenantId, cancellationToken);
        return tenant is null ? TypedResults.NotFound() : TypedResults.Ok(tenant);
    }

    private static async Task<Created<TenantDto>> CreateAsync(
        TenantDto tenant, ITenantService tenantService, CancellationToken cancellationToken)
    {
        var created = await tenantService.CreateAsync(tenant, cancellationToken);
        return TypedResults.Created($"{ApiRoutes.Tenants.Group}/{created.Id}", created);
    }

    private static async Task<NoContent> UpdateAsync(
        TenantDto tenant, ITenantService tenantService, CancellationToken cancellationToken)
    {
        await tenantService.UpdateAsync(tenant, cancellationToken);
        return TypedResults.NoContent();
    }

    private static async Task<NoContent> DeleteAsync(
        Guid id, ITenantService tenantService, CancellationToken cancellationToken)
    {
        await tenantService.DeleteAsync(id, cancellationToken);
        return TypedResults.NoContent();
    }
}
