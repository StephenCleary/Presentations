namespace WebApi.Services;

public class WeatherForecastService(ILogger<WeatherForecastService> logger)
{
	private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
	];

	public WeatherForecast GetWeatherForecast(DateOnly date)
	{
        // [Demo 3.1]
        //using var _ = logger.BeginScope(new Dictionary<string, object>() { { "DateRequested", date } });

        // [Demo 4.1]
        //if (date == DateOnly.FromDateTime(DateTime.Now.AddDays(3)))
        //	throw new InvalidOperationException("Oh no! No temperature available!");

        var temperature = Random.Shared.Next(-20, 55);
        // [Demo 2]
        //logger.LogInformation("Forecast result: {temperature}", temperature);

        return new WeatherForecast
		{
			Date = date,
			TemperatureC = temperature,
			Summary = Summaries[Random.Shared.Next(Summaries.Length)]
		};
	}
}
