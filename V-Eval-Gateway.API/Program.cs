using V_Eval_Gateway.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1. Setup CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 2. Add YARP Reverse Proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// 3. Add OpenAPI & Health Checks
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

var app = builder.Build();

// 4. Configure OpenAPI in Development
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// 5. Use Modular Gateway Middleware Pipeline
app.UseGatewayMiddlewarePipeline();

// 6. Gateway Health Check Endpoint
app.MapGet("/healthz", () => Results.Ok(new 
{ 
    Status = "Healthy", 
    Service = "V-Eval API Gateway (YARP)", 
    Timestamp = DateTime.UtcNow 
})).WithName("GatewayHealthCheck");

// 7. Map YARP Reverse Proxy Routes
app.MapReverseProxy();

app.Run();
