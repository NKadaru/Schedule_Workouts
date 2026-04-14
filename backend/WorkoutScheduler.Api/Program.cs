using System.Text.Json;
using Scalar.AspNetCore;
using WorkoutScheduler.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Register application services
builder.Services.AddScoped<WeatherForecastService>();
builder.Services.AddScoped<WorkoutService>();
builder.Services.AddHttpClient<GeminiAgentService>();
builder.Services.AddHttpClient<WhoopService>();

// Add controller support with camelCase JSON serialization
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Add OpenAPI document generation
builder.Services.AddOpenApi();

// Configure CORS to allow the Angular dev server origin
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Enable OpenAPI + Scalar interactive docs
app.MapOpenApi();
app.MapScalarApiReference();

app.UseCors();

app.MapControllers();

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
