using Nito.Logging;
using OpenTelemetry;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Use .NET Aspire telemetry defaults.
// Hmmm... I wonder what this does?
//builder.ConfigureOpenTelemetry();

// Export to local OTLP.
//builder.Services.AddOpenTelemetry().UseOtlpExporter();

//builder.Services.Decorate<ILoggerProvider, ExceptionLoggingScopeProvider>();
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
