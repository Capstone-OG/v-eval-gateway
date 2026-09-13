using Microsoft.AspNetCore.Builder;
using V_Eval_Gateway.API.Middlewares;

namespace V_Eval_Gateway.API.Extensions;

public static class MiddlewarePipelineExtensions
{
    public static IApplicationBuilder UseGatewayMiddlewarePipeline(this IApplicationBuilder app)
    {
        // 1. Exception handling at the outermost boundary
        app.UseMiddleware<GlobalExceptionMiddleware>();

        // 2. Correlation ID injection & propagation for distributed tracing
        app.UseMiddleware<CorrelationIdMiddleware>();

        // 3. Structured Request/Response logging
        app.UseMiddleware<RequestLoggingMiddleware>();

        // 4. Global CORS policy
        app.UseCors("AllowAll");

        return app;
    }
}
