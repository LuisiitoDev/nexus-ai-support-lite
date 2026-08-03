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
            .WithName(OpenApiMetadata.TenantMemberships.GetAllName)
            .WithSummary(OpenApiMetadata.TenantMemberships.GetAllSummary)
            .WithDescription(OpenApiMetadata.TenantMemberships.GetAllDescription);

        group.MapGet(ApiRoutes.TenantMemberships.ById, GetByIdAsync)
            .WithName(OpenApiMetadata.TenantMemberships.GetByIdName)
            .WithSummary(OpenApiMetadata.TenantMemberships.GetByIdSummary)
            .WithDescription(OpenApiMetadata.TenantMemberships.GetByIdDescription);

        group.MapGet(ApiRoutes.TenantMemberships.ByUser, GetByUserIdAsync)
            .WithName(OpenApiMetadata.TenantMemberships.GetByUserName)
            .WithSummary(OpenApiMetadata.TenantMemberships.GetByUserSummary)
            .WithDescription(OpenApiMetadata.TenantMemberships.GetByUserDescription);

        group.MapGet(ApiRoutes.TenantMemberships.ByTenantAndUser, GetByTenantAndUserAsync)
            .WithName(OpenApiMetadata.TenantMemberships.GetByTenantAndUserName)
            .WithSummary(OpenApiMetadata.TenantMemberships.GetByTenantAndUserSummary)
            .WithDescription(OpenApiMetadata.TenantMemberships.GetByTenantAndUserDescription);

        group.MapPost("/", CreateAsync)
            .WithName(OpenApiMetadata.TenantMemberships.CreateName)
            .WithSummary(OpenApiMetadata.TenantMemberships.CreateSummary)
            .WithDescription(OpenApiMetadata.TenantMemberships.CreateDescription);

        group.MapPut("/", UpdateAsync)
            .WithName(OpenApiMetadata.TenantMemberships.UpdateName)
            .WithSummary(OpenApiMetadata.TenantMemberships.UpdateSummary)
            .WithDescription(OpenApiMetadata.TenantMemberships.UpdateDescription);

        group.MapDelete(ApiRoutes.TenantMemberships.ById, DeleteAsync)
            .WithName(OpenApiMetadata.TenantMemberships.DeleteName)
            .WithSummary(OpenApiMetadata.TenantMemberships.DeleteSummary)
            .WithDescription(OpenApiMetadata.TenantMemberships.DeleteDescription);

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
        TenantMembershipDto tenantMembership, ITenantMembershipService tenantMembershipService, CancellationToken cancellationToken)
    {
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
