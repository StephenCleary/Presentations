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
//builder.Services.AddOpenTelemetry()
//    .WithTracing(tracing => tracing.ConfigureResource(resource => resource.AddService("TelDemo.WebApi")))
//    .WithMetrics(metrics => metrics.ConfigureResource(resource => resource.AddService("TelDemo.WebApi")))
//    .WithLogging(logging => logging.ConfigureResource(resource => resource.AddService("TelDemo.WebApi")));

// [Demo 1.3]
// Third-party libraries can tie into telemetry, too!
//builder.Services.AddOpenTelemetry()
//    .WithTracing(tracing => tracing
//        .AddAWSInstrumentation()
//        .AddSource("RabbitMQ.*")
//        .AddSource("MySqlConnector"))
//    .WithMetrics(metrics => metrics.AddMeter("MySqlConnector"));

// [Demo 5.1]
// Export our custom metrics to OTLP.
//builder.Services.AddOpenTelemetry()
//    .WithMetrics(metrics => metrics.AddMeter("TelDemo.*"));

// [Demo 6.1]
// Export our custom traces to OTLP.
//builder.Services.AddOpenTelemetry()
//    .WithTracing(tracing => tracing.AddSource("TelDemo.*"));

// [Demo 4.2]
//builder.Services.AddExceptionLoggingScopes();

builder.Services.AddMySqlDataSource("Server=localhost;Port=3306;Database=teldemo;Uid=teldemo_user;Pwd=teldemo_password;");
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<RabbitMqPublisherService>();
builder.Services.AddSingleton<SqsPublisherService>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseMiddleware<FakeUserAuthenticationMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();
