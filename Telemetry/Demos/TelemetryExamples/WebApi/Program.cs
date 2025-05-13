using Nito.Logging;
using OpenTelemetry;
using WebApi.Services;

// Avoid delays reporting telemtry (only do this for demo code).
Environment.SetEnvironmentVariable("OTEL_METRIC_EXPORT_INTERVAL", "1000");
Environment.SetEnvironmentVariable("OTEL_BSP_SCHEDULE_DELAY", "1000");
Environment.SetEnvironmentVariable("OTEL_BLRP_SCHEDULE_DELAY", "1000");

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
