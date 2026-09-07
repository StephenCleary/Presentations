using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TelDemo.Frontend.Models;
using TelDemo.Frontend.Services;

namespace TelDemo.Frontend.Pages;

public class IndexModel(WeatherApiClient weatherApiClient) : PageModel
{
    public IReadOnlyList<WeatherForecast> Forecasts { get; private set; } = [];
    public string? TraceId { get; private set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Forecasts = await weatherApiClient.GetForecastsAsync(cancellationToken);
        TraceId = Activity.Current?.TraceId.ToString();
    }
}
