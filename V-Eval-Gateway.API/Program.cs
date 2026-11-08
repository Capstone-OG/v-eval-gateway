using V_Eval_Gateway.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 1. Add All Gateway Services (CORS, YARP, Transforms, RateLimiting, Auth, HealthChecks)
builder.Services.AddGatewayServices(builder.Configuration);

var app = builder.Build();

// 2. Use Modular Gateway Middleware Pipeline
app.UseGatewayMiddlewarePipeline();

// 3. Configure OpenAPI & Unified Gateway Swagger UI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "V-Eval API Gateway - Unified Swagger Portal";
    c.SwaggerEndpoint("/swagger-docs/practice/v1/swagger.json", "1. Practice Service API (Core Flows 1 - 6)");
    c.SwaggerEndpoint("/swagger-docs/identity/v1/swagger.json", "2. Identity Service API (Auth & JWT)");
    c.SwaggerEndpoint("/swagger-docs/content/v1/swagger.json", "3. Content Service API (Question Bank & Exams)");
    c.SwaggerEndpoint("/swagger-docs/ai-engine/v1/swagger.json", "4. AI Engine .NET API (OCR & Ingestion)");
});

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
