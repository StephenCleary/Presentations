using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(WeatherForecastService service) : ControllerBase
{
	[HttpGet]
	public IEnumerable<WeatherForecast> Get()
	{
		var today = DateOnly.FromDateTime(DateTime.Now);
		return Enumerable.Range(1, 5)
			.Select(index => service.GetWeatherForecast(today.AddDays(index)))
			.ToArray();
	}
}
