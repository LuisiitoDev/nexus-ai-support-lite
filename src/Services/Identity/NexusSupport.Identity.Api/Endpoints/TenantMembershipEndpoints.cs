using Microsoft.AspNetCore.Http.HttpResults;
using NexusSupport.Identity.Api.Constants;
using NexusSupport.Identity.Application.Dtos;
using NexusSupport.Identity.Application.Interfaces;

namespace NexusSupport.Identity.Api.Endpoints;

public static class TenantMembershipEndpoints
{
    public static IEndpointRouteBuilder MapTenantMembershipEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.TenantMemberships.Group)
            .WithTags(OpenApiMetadata.Tags.TenantMemberships);

        group.MapGet("/", GetAllAsync)
            .WithName("GetTenantMemberships")
            .WithSummary("Lists tenant memberships")
            .WithDescription("Returns every membership linking a user to an organization.");

        group.MapGet(ApiRoutes.TenantMemberships.ById, GetByIdAsync)
            .WithName("GetTenantMembershipById")
            .WithSummary("Gets a tenant membership by id")
            .WithDescription("Returns a single membership by its Nexus-local identifier.");

        group.MapGet(ApiRoutes.TenantMemberships.ByUser, GetByUserIdAsync)
            .WithName("GetTenantMembershipsByUser")
            .WithSummary("Lists a user's tenant memberships")
            .WithDescription("Returns every organization a given user belongs to.");

        group.MapGet(ApiRoutes.TenantMemberships.ByTenantAndUser, GetByTenantAndUserAsync)
            .WithName("GetTenantMembershipByTenantAndUser")
            .WithSummary("Gets a user's membership within an organization")
            .WithDescription("Returns the membership linking the given user to the given organization, if one exists.");

        group.MapPost("/", CreateAsync)
            .WithName("CreateTenantMembership")
            .WithSummary("Creates a tenant membership")
            .WithDescription("Links a user to an organization, e.g. on first valid sign-in as described in ADR-002.");

        group.MapPut(ApiRoutes.TenantMemberships.ById, UpdateAsync)
            .WithName("UpdateTenantMembership")
            .WithSummary("Updates a tenant membership")
            .WithDescription("Updates the status of an existing tenant membership.");

        group.MapDelete(ApiRoutes.TenantMemberships.ById, DeleteAsync)
            .WithName("DeleteTenantMembership")
            .WithSummary("Deletes a tenant membership")
            .WithDescription("Permanently removes the link between a user and an organization.");

        return app;
    }

    private static async Task<Ok<IReadOnlyList<TenantMembershipDto>>> GetAllAsync(
        ITenantMembershipService tenantMembershipService, CancellationToken cancellationToken)
        => TypedResults.Ok(await tenantMembershipService.GetAllAsync(cancellationToken));

    private static async Task<Results<Ok<TenantMembershipDto>, NotFound>> GetByIdAsync(
        Guid id, ITenantMembershipService tenantMembershipService, CancellationToken cancellationToken)
    {
        var membership = await tenantMembershipService.GetByIdAsync(id, cancellationToken);
        return membership is null ? TypedResults.NotFound() : TypedResults.Ok(membership);
    }

    private static async Task<Ok<IReadOnlyList<TenantMembershipDto>>> GetByUserIdAsync(
        Guid userId, ITenantMembershipService tenantMembershipService, CancellationToken cancellationToken)
        => TypedResults.Ok(await tenantMembershipService.GetByUserIdAsync(userId, cancellationToken));

    private static async Task<Results<Ok<TenantMembershipDto>, NotFound>> GetByTenantAndUserAsync(
        Guid tenantId, Guid userId, ITenantMembershipService tenantMembershipService, CancellationToken cancellationToken)
    {
        var membership = await tenantMembershipService.GetByTenantAndUserAsync(tenantId, userId, cancellationToken);
        return membership is null ? TypedResults.NotFound() : TypedResults.Ok(membership);
    }

    private static async Task<Created<TenantMembershipDto>> CreateAsync(
        TenantMembershipDto tenantMembership, ITenantMembershipService tenantMembershipService, CancellationToken cancellationToken)
    {
        var created = await tenantMembershipService.CreateAsync(tenantMembership, cancellationToken);
        return TypedResults.Created($"{ApiRoutes.TenantMemberships.Group}/{created.Id}", created);
    }

    private static async Task<NoContent> UpdateAsync(
        Guid id, TenantMembershipDto tenantMembership, ITenantMembershipService tenantMembershipService, CancellationToken cancellationToken)
    {
        tenantMembership.Id = id;
        await tenantMembershipService.UpdateAsync(tenantMembership, cancellationToken);
        return TypedResults.NoContent();
    }

    private static async Task<NoContent> DeleteAsync(
        Guid id, ITenantMembershipService tenantMembershipService, CancellationToken cancellationToken)
    {
        await tenantMembershipService.DeleteAsync(id, cancellationToken);
        return TypedResults.NoContent();
    }
}
