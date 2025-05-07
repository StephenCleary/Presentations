using Demo3;
using Nito.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<QueueWorker>();

builder.Services.AddLogging(logging =>
{
	logging.AddSimpleConsole(options => options.IncludeScopes = true);
});

builder.Services.AddExceptionLoggingScopes();

var host = builder.Build();
host.Run();
