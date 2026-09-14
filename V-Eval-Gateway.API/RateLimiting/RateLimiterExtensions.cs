using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;

namespace V_Eval_Gateway.API.RateLimiting;

/// <summary>
/// Extension methods for setting up Rate Limiting policies to protect downstream microservices from abuse/DDoS.
/// </summary>
public static class RateLimiterExtensions
{
    public const string FixedWindowPolicy = "fixed-window";
    public const string SlidingWindowPolicy = "sliding-window";

    public static IServiceCollection AddGatewayRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // 1. Fixed Window Policy (e.g., 100 requests per 1 minute)
            options.AddFixedWindowLimiter(policyName: FixedWindowPolicy, fixedOptions =>
            {
                fixedOptions.PermitLimit = 100;
                fixedOptions.Window = TimeSpan.FromMinutes(1);
                fixedOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                fixedOptions.QueueLimit = 10;
            });

            // 2. Sliding Window Policy (e.g., 60 requests per minute with 6 segments)
            options.AddSlidingWindowLimiter(policyName: SlidingWindowPolicy, slidingOptions =>
            {
                slidingOptions.PermitLimit = 60;
                slidingOptions.Window = TimeSpan.FromMinutes(1);
                slidingOptions.SegmentsPerWindow = 6;
                slidingOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                slidingOptions.QueueLimit = 5;
            });
        });

        return services;
    }
}
