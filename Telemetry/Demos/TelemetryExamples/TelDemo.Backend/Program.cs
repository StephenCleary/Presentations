var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource("TelDemo.*"))
    .UseOtlpExporter();

builder.Services.AddSingleton<ReportGeneratorService>();
builder.Services.AddHostedService<RabbitMqConsumerWorker>();

var host = builder.Build();
host.Run();
