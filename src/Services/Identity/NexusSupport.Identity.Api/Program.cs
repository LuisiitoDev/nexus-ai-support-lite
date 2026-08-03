using Microsoft.OpenApi;
using NexusSupport.Identity.Api.Constants;
using NexusSupport.Identity.Api.Endpoints;
using NexusSupport.Identity.Application.Extensions;
using NexusSupport.Identity.Infrastructure.Extensions;
using Scalar.AspNetCore;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = OpenApiMetadata.Document.Title,
            Version = OpenApiMetadata.Document.Version,
            Description = OpenApiMetadata.Document.Description
        };
        return Task.CompletedTask;
    });
});
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference("/docs", options =>
    {
        options
            .WithTitle("Nexus Identity")
            .WithTheme(ScalarTheme.Mars)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health");

app.MapUserEndpoints();
app.MapTenantEndpoints();
app.MapTenantMembershipEndpoints();
app.MapIdentityProviderEndpoints();
app.MapRolEndpoints();
app.MapMembershipRoleEndpoints();

await app.RunAsync();
