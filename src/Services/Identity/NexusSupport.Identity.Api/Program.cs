using NexusSupport.Identity.Api.Constants;
using NexusSupport.Identity.Api.Endpoints;
using NexusSupport.Identity.Application.Extensions;
using NexusSupport.Identity.Infrastructure.Extensions;
using Scalar.AspNetCore;
var builder = WebApplication.CreateBuilder(args);

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

app.MapHealthChecks("/health");

app.MapUserEndpoints();
app.MapTenantEndpoints();
app.MapTenantMembershipEndpoints();
app.MapRolEndpoints();
app.MapMembershipRoleEndpoints();

await app.RunAsync();
