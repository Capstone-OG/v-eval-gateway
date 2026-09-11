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

// 4. Configure HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");

// Correlation ID Middleware for Distributed Tracing
app.Use(async (context, next) =>
{
    const string correlationIdHeader = "X-Correlation-ID";
    if (!context.Request.Headers.ContainsKey(correlationIdHeader))
    {
        context.Request.Headers[correlationIdHeader] = Guid.NewGuid().ToString();
    }
    context.Response.Headers[correlationIdHeader] = context.Request.Headers[correlationIdHeader];
    await next();
});

// Gateway Health Check Endpoint
app.MapGet("/healthz", () => Results.Ok(new 
{ 
    Status = "Healthy", 
    Service = "V-Eval API Gateway (YARP)", 
    Timestamp = DateTime.UtcNow 
})).WithName("GatewayHealthCheck");

// Map YARP Reverse Proxy Routes
app.MapReverseProxy();

app.Run();
