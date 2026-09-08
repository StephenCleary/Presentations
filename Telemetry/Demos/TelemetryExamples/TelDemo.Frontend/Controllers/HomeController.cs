namespace TelDemo.Frontend.Controllers;

public class HomeController(WeatherApiClient weatherApiClient, RabbitMqPublisherService rabbitMqPublisherService) : Controller
{
    private static readonly ActivitySource ActivitySource = new("TelDemo.Frontend.Controller");

    [HttpGet("/")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity("RenderFrontendHome");
        return await activity.Execute(async () =>
        {
            var model = await BuildModelAsync(cancellationToken);
            return View("~/Pages/Index.cshtml", model);
        });
    }

    [HttpPost("/publish-message")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PublishMessage(CancellationToken cancellationToken)
    {
        using var activity = ActivitySource.StartActivity("PublishFromFrontend");
        return await activity.Execute(async () =>
        {
            var messageId = await rabbitMqPublisherService.PublishGenerateWeatherReportMessageAsync();
            var model = await BuildModelAsync(cancellationToken, messageId);
            return View("~/Pages/Index.cshtml", model);
        });
    }

    private async Task<HomePageViewModel> BuildModelAsync(CancellationToken cancellationToken, string? rabbitMqMessageId = null)
    {
        var forecasts = await weatherApiClient.GetForecastsAsync(cancellationToken);
        return new HomePageViewModel
        {
            Forecasts = forecasts,
            RabbitMqMessageId = rabbitMqMessageId
        };
    }
}
