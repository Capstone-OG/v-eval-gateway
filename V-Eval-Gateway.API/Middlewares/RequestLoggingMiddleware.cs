using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace V_Eval_Gateway.API.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var method = context.Request.Method;
        var path = context.Request.Path;
        var queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty;

        _logger.LogInformation("HTTP {Method} {Path}{QueryString} initiated. TraceId: {TraceId}",
            method, path, queryString, context.TraceIdentifier);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            if (statusCode >= 500)
            {
                _logger.LogError("HTTP {Method} {Path} completed with status {StatusCode} in {ElapsedMs}ms. TraceId: {TraceId}",
                    method, path, statusCode, elapsedMs, context.TraceIdentifier);
            }
            else if (statusCode >= 400)
            {
                _logger.LogWarning("HTTP {Method} {Path} completed with status {StatusCode} in {ElapsedMs}ms. TraceId: {TraceId}",
                    method, path, statusCode, elapsedMs, context.TraceIdentifier);
            }
            else
            {
                _logger.LogInformation("HTTP {Method} {Path} completed with status {StatusCode} in {ElapsedMs}ms. TraceId: {TraceId}",
                    method, path, statusCode, elapsedMs, context.TraceIdentifier);
            }
        }
    }
}
