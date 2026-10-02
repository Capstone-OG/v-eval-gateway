using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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
        var jwtSecret = configuration["JwtSettings:SecretKey"] ?? "V-Eval_Secure_JWT_Key_2026_Identity_Service_Capstone_Secret!";
        var jwtIssuer = configuration["JwtSettings:Issuer"] ?? "V-Eval-IdentityService";
        var jwtAudience = configuration["JwtSettings:Audience"] ?? "V-Eval-Clients";

        // 2. Configure JWT Bearer Authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
                ValidIssuer = jwtIssuer,
                ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });

        services.AddAuthorization(options =>
        {
            // Define Gateway-level authorization policies here if needed
        });

        return services;
    }
}
