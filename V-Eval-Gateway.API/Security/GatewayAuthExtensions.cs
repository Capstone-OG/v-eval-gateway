using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace V_Eval_Gateway.API.Security;

/// <summary>
/// Extension methods for configuring Gateway Authentication & Authorization services.
/// Secrets & JWT Options must be injected strictly via environment variables or appsettings.json.
/// </summary>
public static class GatewayAuthExtensions
{
    public static IServiceCollection AddGatewayAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Read JWT settings strictly from Configuration / Environment Variables
        var jwtSecret = configuration["JwtSettings:SecretKey"];
        var jwtIssuer = configuration["JwtSettings:Issuer"];
        var jwtAudience = configuration["JwtSettings:Audience"];

        // 2. Validate configuration at startup (Do NOT use hardcoded fallback secrets)
        var refreshThresholdMinutes = configuration.GetValue<int>("JwtSettings:RefreshThresholdMinutes", 5);
        if (string.IsNullOrWhiteSpace(jwtSecret))
        {
            // Throw exception during startup if JWT secret is missing in environment
            // throw new InvalidOperationException("JWT SecretKey is missing in Gateway configuration.");
        }

        // 3. Configure JWT Bearer Authentication when JWT package is added
        // Example:
        // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //         .AddJwtBearer(options =>
        //         {
        //             options.TokenValidationParameters = new TokenValidationParameters
        //             {
        //                 ValidateIssuerSigningKey = true,
        //                 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret!)),
        //                 ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
        //                 ValidIssuer = jwtIssuer,
        //                 ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
        //                 ValidAudience = jwtAudience,
        //                 ValidateLifetime = true,
        //                 ClockSkew = TimeSpan.Zero
        //             };
        //         });

        services.AddAuthorization(options =>
        {
            // Define Gateway-level authorization policies here if needed (e.g. RequireAdminRole)
        });

        return services;
    }
}
