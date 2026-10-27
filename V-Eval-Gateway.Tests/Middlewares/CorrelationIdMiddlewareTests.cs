using Microsoft.AspNetCore.Http;
using V_Eval_Gateway.API.Middlewares;
using Xunit;

namespace V_Eval_Gateway.Tests.Middlewares;

public class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WhenCorrelationIdMissing_GeneratesNewGuidAndSetsHeaders()
    {
        // Arrange
        var context = new DefaultHttpContext();
        bool nextCalled = false;
        RequestDelegate next = (ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };
        var middleware = new CorrelationIdMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
        Assert.True(context.Request.Headers.ContainsKey(CorrelationIdMiddleware.CorrelationIdHeader));
        var generatedId = context.Request.Headers[CorrelationIdMiddleware.CorrelationIdHeader].ToString();
        Assert.False(string.IsNullOrWhiteSpace(generatedId));
        Assert.True(Guid.TryParse(generatedId, out _));
        Assert.Equal(generatedId, context.TraceIdentifier);
    }

    [Fact]
    public async Task InvokeAsync_WhenCorrelationIdExists_PreservesExistingCorrelationId()
    {
        // Arrange
        var existingCorrelationId = "test-correlation-id-12345";
        var context = new DefaultHttpContext();
        context.Request.Headers[CorrelationIdMiddleware.CorrelationIdHeader] = existingCorrelationId;

        bool nextCalled = false;
        RequestDelegate next = (ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };
        var middleware = new CorrelationIdMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
        Assert.Equal(existingCorrelationId, context.Request.Headers[CorrelationIdMiddleware.CorrelationIdHeader].ToString());
        Assert.Equal(existingCorrelationId, context.TraceIdentifier);
    }

    [Fact]
    public async Task InvokeAsync_AlwaysExecutesNextDelegate()
    {
        // Arrange
        var context = new DefaultHttpContext();
        int executionCount = 0;
        RequestDelegate next = (ctx) =>
        {
            executionCount++;
            return Task.CompletedTask;
        };
        var middleware = new CorrelationIdMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(1, executionCount);
    }
}
