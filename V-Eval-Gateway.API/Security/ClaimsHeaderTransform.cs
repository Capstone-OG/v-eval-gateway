using Yarp.ReverseProxy.Transforms;

namespace V_Eval_Gateway.API.Security;

/// <summary>
/// Transform for extracting authenticated user claims and injecting them into downstream HTTP headers.
/// Also notifies Frontend via response headers when JWT Token is expiring soon based on Admin settings.
/// </summary>
public static class ClaimsHeaderTransform
{
    private const string HeaderUserId = "X-User-Id";
    private const string HeaderUserRole = "X-User-Role";
    private const string HeaderUserEmail = "X-User-Email";
    private const string HeaderTokenExpiresAt = "X-Token-Expires-At";
    private const string HeaderTokenRemainingSeconds = "X-Token-Remaining-Seconds";
    private const string ResponseHeaderRefreshRequired = "X-Token-Refresh-Required";

    public static TransformBuilderContext AddClaimsTransform(this TransformBuilderContext context)
    {
        context.AddRequestTransform(transformContext =>
        {
            var httpContext = transformContext.HttpContext;
            var user = httpContext.User;

            // Always sanitize incoming request headers to prevent spoofing
            var headers = transformContext.ProxyRequest.Headers;
            headers.Remove(HeaderUserId);
            headers.Remove(HeaderUserRole);
            headers.Remove(HeaderUserEmail);
            headers.Remove(HeaderTokenExpiresAt);
            headers.Remove(HeaderTokenRemainingSeconds);

            if (user.Identity?.IsAuthenticated == true)
            {
                // 1. Extract User Identity Claims
                var userId = user.FindFirst("sub")?.Value ?? user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var userRole = user.FindFirst("role")?.Value ?? user.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                var userEmail = user.FindFirst("email")?.Value ?? user.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    headers.Add(HeaderUserId, userId);
                }

                if (!string.IsNullOrEmpty(userRole))
                {
                    headers.Add(HeaderUserRole, userRole);
                }

                if (!string.IsNullOrEmpty(userEmail))
                {
                    headers.Add(HeaderUserEmail, userEmail);
                }

                // 2. Extract Token Expiration Claim ('exp' unix timestamp in seconds)
                var expClaim = user.FindFirst("exp")?.Value;
                if (!string.IsNullOrEmpty(expClaim) && long.TryParse(expClaim, out var expUnixSeconds))
                {
                    var expiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(expUnixSeconds).UtcDateTime;
                    var remainingSeconds = Math.Max(0, (long)(expiresAtUtc - DateTime.UtcNow).TotalSeconds);

                    headers.Add(HeaderTokenExpiresAt, expiresAtUtc.ToString("o")); // ISO 8601 UTC format
                    headers.Add(HeaderTokenRemainingSeconds, remainingSeconds.ToString());

                    // 3. Admin Configurable Warning: Notify Frontend via Response Header if expiring soon (e.g. < 5 minutes = 300s)
                    var config = httpContext.RequestServices.GetService(typeof(Microsoft.Extensions.Configuration.IConfiguration)) as Microsoft.Extensions.Configuration.IConfiguration;
                    var refreshThresholdMinutes = config?.GetValue<int>("JwtSettings:RefreshThresholdMinutes", 5) ?? 5;
                    var thresholdSeconds = refreshThresholdMinutes * 60;

                    if (remainingSeconds > 0 && remainingSeconds <= thresholdSeconds)
                    {
                        httpContext.Response.OnStarting(() =>
                        {
                            if (!httpContext.Response.Headers.ContainsKey(ResponseHeaderRefreshRequired))
                            {
                                httpContext.Response.Headers[ResponseHeaderRefreshRequired] = "true";
                            }
                            return Task.CompletedTask;
                        });
                    }
                }
            }

            return ValueTask.CompletedTask;
        });

        return context;
    }
}
