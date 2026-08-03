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
            .WithName(OpenApiMetadata.IdentityProviders.GetAllName)
            .WithSummary(OpenApiMetadata.IdentityProviders.GetAllSummary)
            .WithDescription(OpenApiMetadata.IdentityProviders.GetAllDescription);

        group.MapGet(ApiRoutes.IdentityProviders.ById, GetByIdAsync)
            .WithName(OpenApiMetadata.IdentityProviders.GetByIdName)
            .WithSummary(OpenApiMetadata.IdentityProviders.GetByIdSummary)
            .WithDescription(OpenApiMetadata.IdentityProviders.GetByIdDescription);

        group.MapGet(ApiRoutes.IdentityProviders.ByTenant, GetByTenantIdAsync)
            .WithName(OpenApiMetadata.IdentityProviders.GetByTenantName)
            .WithSummary(OpenApiMetadata.IdentityProviders.GetByTenantSummary)
            .WithDescription(OpenApiMetadata.IdentityProviders.GetByTenantDescription);

        group.MapPost("/", CreateAsync)
            .WithName(OpenApiMetadata.IdentityProviders.CreateName)
            .WithSummary(OpenApiMetadata.IdentityProviders.CreateSummary)
            .WithDescription(OpenApiMetadata.IdentityProviders.CreateDescription);

        group.MapPut("/", UpdateAsync)
            .WithName(OpenApiMetadata.IdentityProviders.UpdateName)
            .WithSummary(OpenApiMetadata.IdentityProviders.UpdateSummary)
            .WithDescription(OpenApiMetadata.IdentityProviders.UpdateDescription);

        group.MapDelete(ApiRoutes.IdentityProviders.ById, DeleteAsync)
            .WithName(OpenApiMetadata.IdentityProviders.DeleteName)
            .WithSummary(OpenApiMetadata.IdentityProviders.DeleteSummary)
            .WithDescription(OpenApiMetadata.IdentityProviders.DeleteDescription);

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
        IdentityProviderDto identityProvider, IIdentityProviderService identityProviderService, CancellationToken cancellationToken)
    {
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
