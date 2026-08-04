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

app.UseHttpsRedirection();

// Identity is not publicly reachable (ADR-002): every "/api" request must carry the Gateway's
// internal shared key. "/health" stays open for the container platform's liveness probe.
app.UseMiddleware<InternalServiceKeyMiddleware>();

app.MapHealthChecks("/health");

app.MapUserEndpoints();
app.MapTenantEndpoints();
app.MapTenantMembershipEndpoints();
app.MapRolEndpoints();
app.MapMembershipRoleEndpoints();

await app.RunAsync();
