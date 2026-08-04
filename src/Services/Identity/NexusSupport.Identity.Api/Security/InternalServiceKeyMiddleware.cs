using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Primitives;

namespace NexusSupport.Identity.Api.Security;

/// <summary>
/// Identity is not publicly reachable and only accepts traffic from the API Gateway (ADR-002).
/// This validates the Gateway's per-service internal shared key on every "/api" request, using a
/// constant-time comparison, and rejects anything else with a generic access-denied response.
/// The key is supplied via configuration (environment variable in non-Development environments,
/// injected by CI/CD) and must never be committed to source control.
/// </summary>
public sealed class InternalServiceKeyMiddleware(RequestDelegate next, IConfiguration configuration)
{
    public const string HeaderName = "X-Internal-Key";
    public const string ConfigurationKey = "InternalServiceKey";

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            await next(context);
            return;
        }

        var expectedKey = configuration[ConfigurationKey];

        if (string.IsNullOrEmpty(expectedKey) || !HasValidKey(context.Request.Headers[HeaderName], expectedKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { title = "Access denied", status = StatusCodes.Status401Unauthorized });
            return;
        }

        await next(context);
    }

    private static bool HasValidKey(StringValues provided, string expectedKey)
    {
        if (provided.Count != 1 || string.IsNullOrEmpty(provided[0]))
            return false;

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(provided[0]!),
            Encoding.UTF8.GetBytes(expectedKey));
    }
}
