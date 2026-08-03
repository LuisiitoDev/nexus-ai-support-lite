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
            .WithName("GetTenants")
            .WithSummary("Lists organizations")
            .WithDescription("Returns every organization (tenant) registered in Nexus.");

        group.MapGet(ApiRoutes.Tenants.ById, GetByIdAsync)
            .WithName("GetTenantById")
            .WithSummary("Gets an organization by id")
            .WithDescription("Returns a single organization (tenant) by its Nexus-local identifier.");

        group.MapPost("/", CreateAsync)
            .WithName("CreateTenant")
            .WithSummary("Registers an organization")
            .WithDescription("Registers a new organization (tenant), per the onboarding flow in ADR-002. The organization is disabled until a Nexus Global Administrator enables it.");

        group.MapPut(ApiRoutes.Tenants.ById, UpdateAsync)
            .WithName("UpdateTenant")
            .WithSummary("Updates an organization")
            .WithDescription("Updates an existing organization's name or enabled status.");

        group.MapDelete(ApiRoutes.Tenants.ById, DeleteAsync)
            .WithName("DeleteTenant")
            .WithSummary("Deletes an organization")
            .WithDescription("Permanently removes an organization (tenant) record.");

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

    private static async Task<Created<TenantDto>> CreateAsync(
        TenantDto tenant, ITenantService tenantService, CancellationToken cancellationToken)
    {
        var created = await tenantService.CreateAsync(tenant, cancellationToken);
        return TypedResults.Created($"{ApiRoutes.Tenants.Group}/{created.Id}", created);
    }

    private static async Task<NoContent> UpdateAsync(
        Guid id, TenantDto tenant, ITenantService tenantService, CancellationToken cancellationToken)
    {
        tenant.Id = id;
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
