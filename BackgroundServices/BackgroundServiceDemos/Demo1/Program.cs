var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();

if (WindowsServiceHelpers.IsWindowsService())
{
	builder.Services.AddSingleton<IHostLifetime, WindowsServiceLifetime>();
	builder.Logging.AddEventLog();
}
else if (SystemdHelpers.IsSystemdService())
{
	builder.Services.AddSingleton<ISystemdNotifier, SystemdNotifier>();
	builder.Services.AddSingleton<IHostLifetime, SystemdLifetime>();
	builder.Logging.AddSystemdConsole(logger => logger.IncludeScopes = true);
}
else
{
	builder.Logging.AddSimpleConsole(logger => logger.IncludeScopes = true);
}

var host = builder.Build();
host.Run();
