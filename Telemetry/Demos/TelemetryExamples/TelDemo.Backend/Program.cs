// Avoid delays reporting telemtry (only do this for demo code).
Environment.SetEnvironmentVariable("OTEL_METRIC_EXPORT_INTERVAL", "1000");
Environment.SetEnvironmentVariable("OTEL_BSP_SCHEDULE_DELAY", "1000");
Environment.SetEnvironmentVariable("OTEL_BLRP_SCHEDULE_DELAY", "1000");

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAWSInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource("TelDemo.*")
        .AddSource("RabbitMQ.*"))
    .UseOtlpExporter();

builder.Services.AddSingleton<ReportGeneratorService>();
builder.Services.AddHostedService<RabbitMqConsumerWorker>();
builder.Services.AddHostedService<SqsConsumerWorker>();

var host = builder.Build();
host.Run();
