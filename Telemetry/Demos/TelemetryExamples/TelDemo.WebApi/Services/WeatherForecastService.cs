namespace TelDemo.WebApi.Services;

public class WeatherForecastService(MySqlDataSource dataSource, ILogger<WeatherForecastService> logger)
{
	// [Demo 5.2]
    //private static readonly Meter Meter = new("TelDemo.WebApi.WeatherForecast");
    //private static readonly Counter<int> ForecastsGenerated = Meter.CreateCounter<int>("weather_forecasts_generated");

	// [Demo 6.2]
    //public static readonly ActivitySource ActivitySource = new("TelDemo.WebApi.WeatherForecast");

	public async Task<WeatherForecast> GetWeatherForecastAsync(DateOnly date)
	{
        // [Demo 3.1]
        //using var _ = logger.BeginScope(new Dictionary<string, object>() { { "DateRequested", date } });

        // [Demo 6.3]
        //using var activity = ActivitySource.StartActivity("GenerateWeatherForecast");
        //activity?.SetTag("weather.date_requested", date.ToString("O"));
        //return activity.Execute(() =>
        //{

        // [Demo 4.1]
        //if (date == DateOnly.FromDateTime(DateTime.Now.AddDays(3)))
        //	throw new InvalidOperationException("Oh no! No temperature available!");

        await using var connection = await dataSource.OpenConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT `temperature_c`, `summary` FROM `temperatures` WHERE `date` = @date LIMIT 1;";
        command.Parameters.AddWithValue("@date", date.ToDateTime(TimeOnly.MinValue));
        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            throw new InvalidOperationException("Could not forecast temperature; recreate the MySql docker container.");

        var tempC = reader.GetInt32(0);
        var summary = reader.GetString(1);

		// [Demo 2]
		//logger.LogInformation("Forecast result: {temperature} ({summary})", tempC, summary);

		// [Demo 5.3]
		//ForecastsGenerated.Add(1);

		return new WeatherForecast
		{
			Date = date,
            TemperatureC = tempC,
            Summary = summary,
        };

        // [Demo 6.4]
        //});
    }
}
