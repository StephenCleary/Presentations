namespace WebApi.Services;

public class WeatherForecastService
{
	private static readonly string[] Summaries = new[]
	{
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
	};

	public WeatherForecast GetWeatherForecast(DateOnly date)
	{
		//using var _ = _logger.BeginScope(new Dictionary<string, object>() { { "DateRequested", date } });

		//if (date == DateOnly.FromDateTime(DateTime.Now.AddDays(3)))
		//	throw new InvalidOperationException("Oh no! No temperature available!");

		var temperature = Random.Shared.Next(-20, 55);
		//_logger.LogInformation("Forecast result: {temperature}", temperature);

		return new WeatherForecast
		{
			Date = date,
			TemperatureC = temperature,
			Summary = Summaries[Random.Shared.Next(Summaries.Length)]
		};
	}

	private readonly ILogger<WeatherForecastService> _logger;
	public WeatherForecastService(ILogger<WeatherForecastService> logger) => _logger = logger;
}
