using Microsoft.AspNetCore.Http.HttpResults;
using NexusSupport.Identity.Api.Constants;
using NexusSupport.Identity.Application.Dtos;
using NexusSupport.Identity.Application.Interfaces;

namespace NexusSupport.Identity.Api.Endpoints;

public static class IdentityProviderEndpoints
{
    public static IEndpointRouteBuilder MapIdentityProviderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.IdentityProviders.Group)
            .WithTags(OpenApiMetadata.Tags.IdentityProviders);

        group.MapGet("/", GetAllAsync)
            .WithName("GetIdentityProviders")
            .WithSummary("Lists identity providers")
            .WithDescription("Returns every configured identity provider across all organizations.");

        group.MapGet(ApiRoutes.IdentityProviders.ById, GetByIdAsync)
            .WithName("GetIdentityProviderById")
            .WithSummary("Gets an identity provider by id")
            .WithDescription("Returns a single identity provider configuration by its Nexus-local identifier.");

        group.MapGet(ApiRoutes.IdentityProviders.ByTenant, GetByTenantIdAsync)
            .WithName("GetIdentityProvidersByTenant")
            .WithSummary("Lists an organization's identity providers")
            .WithDescription("Returns every identity provider configured for a given organization (tenant).");

        group.MapPost("/", CreateAsync)
            .WithName("CreateIdentityProvider")
            .WithSummary("Creates an identity provider")
            .WithDescription("Registers a new identity provider configuration for an organization.");

        group.MapPut(ApiRoutes.IdentityProviders.ById, UpdateAsync)
            .WithName("UpdateIdentityProvider")
            .WithSummary("Updates an identity provider")
            .WithDescription("Updates an existing identity provider's configuration or enabled status.");

        group.MapDelete(ApiRoutes.IdentityProviders.ById, DeleteAsync)
            .WithName("DeleteIdentityProvider")
            .WithSummary("Deletes an identity provider")
            .WithDescription("Permanently removes an identity provider configuration.");

        return app;
    }

    private static async Task<Ok<IReadOnlyList<IdentityProviderDto>>> GetAllAsync(
        IIdentityProviderService identityProviderService, CancellationToken cancellationToken)
        => TypedResults.Ok(await identityProviderService.GetAllAsync(cancellationToken));

    private static async Task<Results<Ok<IdentityProviderDto>, NotFound>> GetByIdAsync(
        Guid id, IIdentityProviderService identityProviderService, CancellationToken cancellationToken)
    {
        var identityProvider = await identityProviderService.GetByIdAsync(id, cancellationToken);
        return identityProvider is null ? TypedResults.NotFound() : TypedResults.Ok(identityProvider);
    }

    private static async Task<Ok<IReadOnlyList<IdentityProviderDto>>> GetByTenantIdAsync(
        Guid tenantId, IIdentityProviderService identityProviderService, CancellationToken cancellationToken)
        => TypedResults.Ok(await identityProviderService.GetByTenantIdAsync(tenantId, cancellationToken));

    private static async Task<Created<IdentityProviderDto>> CreateAsync(
        IdentityProviderDto identityProvider, IIdentityProviderService identityProviderService, CancellationToken cancellationToken)
    {
        var created = await identityProviderService.CreateAsync(identityProvider, cancellationToken);
        return TypedResults.Created($"{ApiRoutes.IdentityProviders.Group}/{created.Id}", created);
    }

    private static async Task<NoContent> UpdateAsync(
        Guid id, IdentityProviderDto identityProvider, IIdentityProviderService identityProviderService, CancellationToken cancellationToken)
    {
        identityProvider.Id = id;
        await identityProviderService.UpdateAsync(identityProvider, cancellationToken);
        return TypedResults.NoContent();
    }

    private static async Task<NoContent> DeleteAsync(
        Guid id, IIdentityProviderService identityProviderService, CancellationToken cancellationToken)
    {
        await identityProviderService.DeleteAsync(id, cancellationToken);
        return TypedResults.NoContent();
    }
}
