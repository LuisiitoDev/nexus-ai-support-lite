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
            .WithName(OpenApiMetadata.MembershipRoles.GetAllName)
            .WithSummary(OpenApiMetadata.MembershipRoles.GetAllSummary)
            .WithDescription(OpenApiMetadata.MembershipRoles.GetAllDescription);

        group.MapGet(ApiRoutes.MembershipRoles.ById, GetByIdAsync)
            .WithName(OpenApiMetadata.MembershipRoles.GetByIdName)
            .WithSummary(OpenApiMetadata.MembershipRoles.GetByIdSummary)
            .WithDescription(OpenApiMetadata.MembershipRoles.GetByIdDescription);

        group.MapGet(ApiRoutes.MembershipRoles.ByTenantMembership, GetByTenantMembershipIdAsync)
            .WithName(OpenApiMetadata.MembershipRoles.GetByTenantMembershipName)
            .WithSummary(OpenApiMetadata.MembershipRoles.GetByTenantMembershipSummary)
            .WithDescription(OpenApiMetadata.MembershipRoles.GetByTenantMembershipDescription);

        group.MapPost("/", CreateAsync)
            .WithName(OpenApiMetadata.MembershipRoles.CreateName)
            .WithSummary(OpenApiMetadata.MembershipRoles.CreateSummary)
            .WithDescription(OpenApiMetadata.MembershipRoles.CreateDescription);

        group.MapPut("/", UpdateAsync)
            .WithName(OpenApiMetadata.MembershipRoles.UpdateName)
            .WithSummary(OpenApiMetadata.MembershipRoles.UpdateSummary)
            .WithDescription(OpenApiMetadata.MembershipRoles.UpdateDescription);

        group.MapDelete(ApiRoutes.MembershipRoles.ById, DeleteAsync)
            .WithName(OpenApiMetadata.MembershipRoles.DeleteName)
            .WithSummary(OpenApiMetadata.MembershipRoles.DeleteSummary)
            .WithDescription(OpenApiMetadata.MembershipRoles.DeleteDescription);

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
        MembershipRoleDto membershipRole, IMembershipRoleService membershipRoleService, CancellationToken cancellationToken)
    {
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
