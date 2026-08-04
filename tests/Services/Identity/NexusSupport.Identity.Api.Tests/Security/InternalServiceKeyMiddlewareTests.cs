using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NexusSupport.Identity.Api.Security;
using Xunit;

namespace NexusSupport.Identity.Api.Tests.Security;

public class InternalServiceKeyMiddlewareTests
{
    private const string ConfiguredKey = "the-real-shared-key";

    [Fact]
    public async Task InvokeAsync_CallsNext_WhenPathIsNotUnderApi()
    {
        var nextCalled = false;
        var middleware = CreateMiddleware(configuredKey: null, _ => { nextCalled = true; return Task.CompletedTask; });
        var context = CreateContext("/health");

        await middleware.InvokeAsync(context);

        Assert.True(nextCalled);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_ReturnsUnauthorized_WhenHeaderIsMissing()
    {
        var nextCalled = false;
        var middleware = CreateMiddleware(ConfiguredKey, _ => { nextCalled = true; return Task.CompletedTask; });
        var context = CreateContext("/api/tenants");

        await middleware.InvokeAsync(context);

        Assert.False(nextCalled);
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_ReturnsUnauthorized_WhenKeyDoesNotMatch()
    {
        var nextCalled = false;
        var middleware = CreateMiddleware(ConfiguredKey, _ => { nextCalled = true; return Task.CompletedTask; });
        var context = CreateContext("/api/tenants", providedKey: "wrong-key");

        await middleware.InvokeAsync(context);

        Assert.False(nextCalled);
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_ReturnsUnauthorized_WhenNoKeyIsConfigured()
    {
        var nextCalled = false;
        var middleware = CreateMiddleware(configuredKey: null, _ => { nextCalled = true; return Task.CompletedTask; });
        var context = CreateContext("/api/tenants", providedKey: "anything");

        await middleware.InvokeAsync(context);

        Assert.False(nextCalled);
        Assert.Equal(StatusCodes.Status401Unauthorized, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_CallsNext_WhenKeyMatches()
    {
        var nextCalled = false;
        var middleware = CreateMiddleware(ConfiguredKey, _ => { nextCalled = true; return Task.CompletedTask; });
        var context = CreateContext("/api/tenants", providedKey: ConfiguredKey);

        await middleware.InvokeAsync(context);

        Assert.True(nextCalled);
        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    private static InternalServiceKeyMiddleware CreateMiddleware(string? configuredKey, RequestDelegate next)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configuredKey is null
                ? []
                : new Dictionary<string, string?> { [InternalServiceKeyMiddleware.ConfigurationKey] = configuredKey })
            .Build();

        return new InternalServiceKeyMiddleware(next, configuration);
    }

    private static DefaultHttpContext CreateContext(string path, string? providedKey = null)
    {
        var context = new DefaultHttpContext();
        context.Request.Path = path;
        context.Response.Body = new MemoryStream();

        if (providedKey is not null)
            context.Request.Headers[InternalServiceKeyMiddleware.HeaderName] = providedKey;

        return context;
    }
}
