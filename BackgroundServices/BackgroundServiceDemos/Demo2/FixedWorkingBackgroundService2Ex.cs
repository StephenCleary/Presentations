namespace Demo2;

public abstract class FixedWorkingBackgroundService2Ex(
	ILogger<FixedWorkingBackgroundService2Ex> logger,
	IHostApplicationLifetime hostApplicationLifetime)
	: BackgroundService
{
	protected ILogger<FixedWorkingBackgroundService2Ex> Logger { get; } = logger;
	protected IHostApplicationLifetime HostApplicationLifetime { get; } = hostApplicationLifetime;

	protected sealed override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Run(async () =>
	{
		try
		{
			await DoExecuteAsync(stoppingToken).ConfigureAwait(false);
			Logger.LogInformation("Worker stopped.");
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, "Worker failed.");
		}
		finally
		{
			HostApplicationLifetime.StopApplication();
		}
	}, CancellationToken.None);

	protected abstract Task DoExecuteAsync(CancellationToken stoppingToken);
}
