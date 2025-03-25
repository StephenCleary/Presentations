using Demo2;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();

// JSON, one line per message:
builder.Logging.AddJsonConsole(logger => logger.IncludeScopes = true);

var host = builder.Build();
host.Run();
