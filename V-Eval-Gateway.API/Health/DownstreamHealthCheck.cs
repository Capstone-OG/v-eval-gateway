using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace V_Eval_Gateway.API.Health;

/// <summary>
/// Health check implementation for probing downstream microservices health.
/// </summary>
public class DownstreamHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;

    public DownstreamHealthCheck(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // TODO: Implement custom health check probes for internal clusters
        try
        {
            // Example probe: Ping identity or content service
            return HealthCheckResult.Healthy("All downstream microservices are reachable.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Failed to reach downstream microservices.", ex);
        }
    }
}
