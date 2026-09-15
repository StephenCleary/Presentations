namespace TelDemo.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(WeatherForecastService service) : ControllerBase
{
	[HttpGet]
	public async Task<IEnumerable<WeatherForecast>> Get(CancellationToken cancellationToken)
	{
		var today = DateOnly.FromDateTime(DateTime.Now);
		List<WeatherForecast> result = new();
		for (var index = 0; index != 5; ++index)
			result.Add(await service.GetWeatherForecastAsync(today.AddDays(index), cancellationToken));
		return result;
	}
}
