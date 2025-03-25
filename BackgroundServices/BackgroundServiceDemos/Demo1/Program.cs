// Work around FileWatcher bug when running as systemd. https://github.com/dotnet/runtime/issues/113855
var config = new ConfigurationManager();
config.AddInMemoryCollection(new Dictionary<string, string?>() { ["hostBuilder:reloadConfigOnChange"] = "false" });
var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
	Args = args,
	Configuration = config,
});

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
