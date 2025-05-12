using Nito.Logging;
using OpenTelemetry;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// [Demo 1.1]
// Use .NET Aspire telemetry defaults.
// Hmmm... I wonder what this does?
//builder.ConfigureOpenTelemetry();

// [Demo 1.2]
// Export to local OTLP.
//builder.Services.AddOpenTelemetry().UseOtlpExporter();

// [Demo 4.2]
//builder.Services.AddExceptionLoggingScopes();

builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseAuthorization();
app.MapControllers();
app.Run();
