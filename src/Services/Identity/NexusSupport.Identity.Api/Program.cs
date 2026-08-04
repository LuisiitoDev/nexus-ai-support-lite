using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using NexusSupport.Identity.Api.Constants;
using NexusSupport.Identity.Api.Endpoints;
using NexusSupport.Identity.Api.Security;
using NexusSupport.Identity.Application.Extensions;
using NexusSupport.Identity.Infrastructure.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Fail at startup rather than on the first request: a missing or too-short key would
// otherwise only surface as a 401 once the Gateway starts calling.
var internalServiceKey = builder.Configuration[InternalServiceKeyMiddleware.ConfigurationKey];
if (string.IsNullOrWhiteSpace(internalServiceKey) || internalServiceKey.Length < 32)
{
    throw new InvalidOperationException(
        $"{InternalServiceKeyMiddleware.ConfigurationKey} must be configured with at least 32 characters.");
}

// Container Apps ingress terminates TLS and forwards plain HTTP to the
// container, so the original scheme is only visible through X-Forwarded-Proto.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddOpenApi(OpenApiMetadata.Document.Name);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(OpenApiMetadata.Document.Route, options =>
    {
        options
            .AddDocument(OpenApiMetadata.Document.Name, OpenApiMetadata.Document.Title)
            .WithTitle(OpenApiMetadata.Document.Title)
            .WithTheme(ScalarTheme.Mars)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseForwardedHeaders();

// Container Apps probes call the container directly over HTTP and send no
// X-Forwarded-Proto, so unconditional HTTPS redirection would answer them with a
// 307. The platform counts 3xx as success, which would let readiness report
// healthy without ever running the database check. Exempt the health endpoints;
// everything else still redirects.
app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/health"),
    branch => branch.UseHttpsRedirection());

// Identity is not publicly reachable (ADR-002): every "/api" request must carry the Gateway's
// internal shared key. "/health" stays open for the container platform's liveness probe.
app.UseMiddleware<InternalServiceKeyMiddleware>();

// Liveness must not depend on the database: a transient SQL outage should not
// cause the orchestrator to restart every otherwise-healthy replica.
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

// Readiness gates ingress traffic on the dependencies the API actually needs.
app.MapHealthChecks("/health/ready");

app.MapHealthChecks("/health");

app.MapUserEndpoints();
app.MapTenantEndpoints();
app.MapTenantMembershipEndpoints();
app.MapRolEndpoints();
app.MapMembershipRoleEndpoints();

await app.RunAsync();
