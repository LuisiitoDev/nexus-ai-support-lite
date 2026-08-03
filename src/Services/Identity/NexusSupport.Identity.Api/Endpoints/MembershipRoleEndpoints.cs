using Microsoft.AspNetCore.Http.HttpResults;
using NexusSupport.Identity.Api.Constants;
using NexusSupport.Identity.Application.Dtos;
using NexusSupport.Identity.Application.Interfaces;

namespace NexusSupport.Identity.Api.Endpoints;

public static class MembershipRoleEndpoints
{
    public static IEndpointRouteBuilder MapMembershipRoleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.MembershipRoles.Group)
            .WithTags(OpenApiMetadata.Tags.MembershipRoles);

        group.MapGet("/", GetAllAsync)
            .WithName("GetMembershipRoles")
            .WithSummary("Lists membership role assignments")
            .WithDescription("Returns every product role assigned to a tenant membership.");

        group.MapGet(ApiRoutes.MembershipRoles.ById, GetByIdAsync)
            .WithName("GetMembershipRoleById")
            .WithSummary("Gets a membership role assignment by id")
            .WithDescription("Returns a single membership role assignment by its Nexus-local identifier.");

        group.MapGet(ApiRoutes.MembershipRoles.ByTenantMembership, GetByTenantMembershipIdAsync)
            .WithName("GetMembershipRolesByTenantMembership")
            .WithSummary("Lists the roles assigned to a tenant membership")
            .WithDescription("Returns every product role assigned to a given tenant membership.");

        group.MapPost("/", CreateAsync)
            .WithName("CreateMembershipRole")
            .WithSummary("Assigns a role to a tenant membership")
            .WithDescription("Assigns a Nexus-managed product role to a tenant membership, e.g. the default Requester role granted on first sign-in per ADR-002.");

        group.MapPut(ApiRoutes.MembershipRoles.ById, UpdateAsync)
            .WithName("UpdateMembershipRole")
            .WithSummary("Updates a membership role assignment")
            .WithDescription("Updates an existing membership role assignment.");

        group.MapDelete(ApiRoutes.MembershipRoles.ById, DeleteAsync)
            .WithName("DeleteMembershipRole")
            .WithSummary("Removes a role from a tenant membership")
            .WithDescription("Permanently removes a product role assignment from a tenant membership.");

        return app;
    }

    private static async Task<Ok<IReadOnlyList<MembershipRoleDto>>> GetAllAsync(
        IMembershipRoleService membershipRoleService, CancellationToken cancellationToken)
        => TypedResults.Ok(await membershipRoleService.GetAllAsync(cancellationToken));

    private static async Task<Results<Ok<MembershipRoleDto>, NotFound>> GetByIdAsync(
        int id, IMembershipRoleService membershipRoleService, CancellationToken cancellationToken)
    {
        var membershipRole = await membershipRoleService.GetByIdAsync(id, cancellationToken);
        return membershipRole is null ? TypedResults.NotFound() : TypedResults.Ok(membershipRole);
    }

    private static async Task<Ok<IReadOnlyList<MembershipRoleDto>>> GetByTenantMembershipIdAsync(
        Guid tenantMembershipId, IMembershipRoleService membershipRoleService, CancellationToken cancellationToken)
        => TypedResults.Ok(await membershipRoleService.GetByTenantMembershipIdAsync(tenantMembershipId, cancellationToken));

    private static async Task<Created<MembershipRoleDto>> CreateAsync(
        MembershipRoleDto membershipRole, IMembershipRoleService membershipRoleService, CancellationToken cancellationToken)
    {
        var created = await membershipRoleService.CreateAsync(membershipRole, cancellationToken);
        return TypedResults.Created($"{ApiRoutes.MembershipRoles.Group}/{created.Id}", created);
    }

    private static async Task<NoContent> UpdateAsync(
        int id, MembershipRoleDto membershipRole, IMembershipRoleService membershipRoleService, CancellationToken cancellationToken)
    {
        membershipRole.Id = id;
        await membershipRoleService.UpdateAsync(membershipRole, cancellationToken);
        return TypedResults.NoContent();
    }

    private static async Task<NoContent> DeleteAsync(
        int id, IMembershipRoleService membershipRoleService, CancellationToken cancellationToken)
    {
        await membershipRoleService.DeleteAsync(id, cancellationToken);
        return TypedResults.NoContent();
    }
}
