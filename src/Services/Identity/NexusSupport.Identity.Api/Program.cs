using System.Security.Cryptography;
using System.Text;
using NexusSupport.Identity.Api.Constants;
using NexusSupport.Identity.Api.Endpoints;
using NexusSupport.Identity.Application.Extensions;
using NexusSupport.Identity.Infrastructure.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var identityServiceKey = builder.Configuration["IdentityServiceKey"];
if (string.IsNullOrWhiteSpace(identityServiceKey) || identityServiceKey.Length < 32)
{
    throw new InvalidOperationException(
        "IdentityServiceKey must be configured with at least 32 characters.");
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

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/health")
    {
        await next(context);
        return;
    }

    var suppliedKeys = context.Request.Headers["X-Identity-Service-Key"];
    var suppliedKey = suppliedKeys.Count == 1 ? suppliedKeys[0] : null;

    if (suppliedKey is null || !ServiceKeysMatch(identityServiceKey, suppliedKey))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new { error = "A valid Identity service key is required." });
        return;
    }

    await next(context);
});

app.MapHealthChecks("/health");

app.MapUserEndpoints();
app.MapTenantEndpoints();
app.MapTenantMembershipEndpoints();
app.MapRolEndpoints();
app.MapMembershipRoleEndpoints();

await app.RunAsync();

static bool ServiceKeysMatch(string expected, string supplied)
{
    var expectedHash = SHA256.HashData(Encoding.UTF8.GetBytes(expected));
    var suppliedHash = SHA256.HashData(Encoding.UTF8.GetBytes(supplied));
    return CryptographicOperations.FixedTimeEquals(expectedHash, suppliedHash);
}
