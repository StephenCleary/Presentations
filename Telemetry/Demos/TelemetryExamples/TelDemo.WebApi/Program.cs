// Avoid delays reporting telemtry (only do this for demo code).
Environment.SetEnvironmentVariable("OTEL_METRIC_EXPORT_INTERVAL", "1000");
Environment.SetEnvironmentVariable("OTEL_BSP_SCHEDULE_DELAY", "1000");
Environment.SetEnvironmentVariable("OTEL_BLRP_SCHEDULE_DELAY", "1000");

var builder = WebApplication.CreateBuilder(args);

// [Demo 1.1]
// Use Aspire telemetry defaults.
// Hmmm... I wonder what this does?
//builder.ConfigureOpenTelemetry();

// [Demo 1.2]
// Export to local OTLP.
//builder.Services.AddOpenTelemetry().UseOtlpExporter();

// [Demo 5]
// Export our custom metrics to OTLP.
//builder.Services.AddOpenTelemetry()
//    .WithMetrics(metrics => metrics.AddMeter("TelDemo.*"));

// [Demo 6]
// Export our custom traces to OTLP.
//builder.Services.AddOpenTelemetry()
//    .WithTracing(tracing => tracing.AddSource("TelDemo.*"));

// [Demo 4.2]
//builder.Services.AddExceptionLoggingScopes();

builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseMiddleware<FakeUserAuthenticationMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();
