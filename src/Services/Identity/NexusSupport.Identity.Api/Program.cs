using NexusSupport.Identity.Api.HealthChecks;
using NexusSupport.Identity.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

var identityConnectionString = builder.Configuration.GetConnectionString("NexusIdentity");
if (string.IsNullOrWhiteSpace(identityConnectionString))
{
    throw new InvalidOperationException(
        "Missing required configuration value 'ConnectionStrings:NexusIdentity'. Identity.Api cannot start without it.");
}

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks()
    .AddCheck<IdentityDatabaseHealthCheck>("identity-database");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health");

await app.RunAsync();
