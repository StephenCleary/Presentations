namespace TelDemo.Frontend.Models;

public class HomePageViewModel
{
    public IReadOnlyList<WeatherForecast> Forecasts { get; init; } = [];
    public string? TraceId { get; init; }
}
