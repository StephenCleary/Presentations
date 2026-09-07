using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherForecastService service) : ControllerBase
{
    [HttpGet]
	public IEnumerable<WeatherForecast> Get()
	{
		var today = DateOnly.FromDateTime(DateTime.Now);
        // [Demo 3.2]
        //using var _ = logger.BeginScope(new Dictionary<string, object>() { { "User", "Demo" } });
        return Enumerable.Range(1, 5)
			.Select(index => service.GetWeatherForecast(today.AddDays(index)))
			.ToArray();
	}
}
