using Log.Services;
using Microsoft.AspNetCore.Mvc;

namespace Log.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly WeatherForecastService _service;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherForecastService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        using var _ = _logger.BeginScope(new Dictionary<string, object>() { { "TodaysDate", today } });
        return Enumerable.Range(1, 5)
            .Select(index => _service.GetWeatherForecast(today.AddDays(index)))
            .ToArray();
    }
}
