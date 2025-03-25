namespace Demo1;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
	private readonly IHostLifetime _hostLifetime;

	public Worker(ILogger<Worker> logger, IHostLifetime hostLifetime)
	{
		_logger = logger;
		_hostLifetime = hostLifetime;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
		using var _1 = _logger.BeginDataScope(("Host", _hostLifetime.GetType().Name));
		
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
