using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using V_Eval_Gateway.API.RateLimiting;
using V_Eval_Gateway.API.Security;
using V_Eval_Gateway.API.Transforms;

namespace V_Eval_Gateway.API.Extensions;

/// <summary>
/// Service collection extension methods to cleanly register all API Gateway dependencies.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGatewayServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Setup CORS Policy
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        // 2. Add YARP Reverse Proxy with Custom Transform Provider
        services.AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"))
            .AddTransforms<GatewayTransformProvider>();

        // 3. Add Gateway Rate Limiting
        services.AddGatewayRateLimiter();

        // 4. Add Gateway Authentication & Authorization
        services.AddGatewayAuthentication(configuration);

        // 5. Add OpenAPI & Health Checks
        services.AddOpenApi();
        services.AddHealthChecks();

        return services;
    }
}
