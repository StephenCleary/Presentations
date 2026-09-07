namespace TelDemo.Frontend.Controllers;

public class HomeController(WeatherApiClient weatherApiClient) : Controller
{
    private static readonly ActivitySource ActivitySource = new("TelDemo.Frontend.Controller");

    [HttpGet("/")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity("RenderFrontendHome");
        var forecasts = await weatherApiClient.GetForecastsAsync(cancellationToken);
        var model = new HomePageViewModel
        {
            Forecasts = forecasts,
            TraceId = Activity.Current?.TraceId.ToString()
        };

        return View("~/Pages/Index.cshtml", model);
    }
}
