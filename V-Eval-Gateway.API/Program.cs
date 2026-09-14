using V_Eval_Gateway.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1. Add All Gateway Services (CORS, YARP, Transforms, RateLimiting, Auth, HealthChecks)
builder.Services.AddGatewayServices(builder.Configuration);

var app = builder.Build();

// 2. Configure OpenAPI in Development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// 3. Use Modular Gateway Middleware Pipeline
app.UseGatewayMiddlewarePipeline();

// 4. Gateway Health Check Endpoint
app.MapGet("/healthz", () => Results.Ok(new 
{ 
    Status = "Healthy", 
    Service = "V-Eval API Gateway (YARP)", 
    Timestamp = DateTime.UtcNow 
})).WithName("GatewayHealthCheck");

// 5. Map YARP Reverse Proxy Routes
app.MapReverseProxy();

app.Run();
