using Nito.Logging;
using StackExchange.Redis;

namespace Demo3;

// docker run --rm -it -p 6379:6379 redis

public class PeriodWorker(ILogger<PeriodWorker> logger,
	IHostApplicationLifetime hostApplicationLifetime,
	IServiceScopeFactory serviceScopeFactory) :
	FixedWorkingBackgroundService2Ex(logger, hostApplicationLifetime)
{
	private static readonly TimeSpan PeriodTime = TimeSpan.FromSeconds(1);
	private static readonly TimeSpan LeaseTime = TimeSpan.FromSeconds(5);
	private static readonly TimeSpan WorkTime = TimeSpan.FromSeconds(2); // Also try 7

	protected override async Task DoExecuteAsync(CancellationToken stoppingToken)
	{
		var id = Guid.NewGuid().ToString("N");
		var redis = await ConnectionMultiplexer.ConnectAsync(new ConfigurationOptions()
		{
			EndPoints = { "localhost:6379" },
		});
		var leaseProvider = new RedisDistributedLeaseProvider(Logger, redis.GetDatabase());

		using var workerLoggingScope = Logger.BeginDataScope(("id", id));

		while (true)
		{
			await Task.Delay(PeriodTime, stoppingToken);

			await using var lease = await leaseProvider.TryAcquireLeaseAsync("demo", LeaseTime);
			if (lease is null)
				continue;

			await using var scope = serviceScopeFactory.CreateAsyncScope();
			var work = DoWorkAsync(scope.ServiceProvider);
			while (true)
			{
				try
				{
					await work.WaitAsync(LeaseTime / 2, stoppingToken);
					break;
				}
				catch (TimeoutException)
				{
					if (!await lease.TryExtendLeaseAsync(LeaseTime))
					{
						Logger.LogWarning("Lease lost!");

						// Alternatively, cancel a cancellation token passed to DoWorkAsync.
						HostApplicationLifetime.StopApplication();
						return;
					}
				}
			}
		}
	}

	// Consider also passing CancellationToken.
	// This example assumes we want to complete processing once we've started.
	private static async Task DoWorkAsync(IServiceProvider serviceProvider)
	{
		// Use the provider to resolve services that may have a per-invocation lifetime.
		var localLogger = serviceProvider.GetRequiredService<ILogger<PeriodWorker>>();
		localLogger.LogInformation("Processing!");
		await Task.Delay(WorkTime);
		localLogger.LogInformation("Done!");
	}
}
