namespace TelDemo.Frontend.Controllers;

public class HomeController(WeatherApiClient weatherApiClient) : Controller
{
    private static readonly ActivitySource ActivitySource = new("TelDemo.Frontend.Controller");

    [HttpGet("/")]
    public async Task<IActionResult> Index()
    {
        using var activity = ActivitySource.StartActivity("RenderFrontendHome");
        return await activity.Execute(async () =>
        {
            var model = await BuildModelAsync();
            return View("~/Pages/Index.cshtml", model);
        });
    }

    [HttpPost("/generate-report-via-rabbitmq")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateReportViaRabbitMq()
    {
        using var activity = ActivitySource.StartActivity("PublishFromFrontend");
        return await activity.Execute(async () =>
        {
            var messageId = await weatherApiClient.GenerateReportViaRabbitMqAsync();
            var model = await BuildModelAsync(rabbitMqMessageId: messageId);
            return View("~/Pages/Index.cshtml", model);
        });
    }

    [HttpPost("/generate-report-via-sqs")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateReportViaSqs()
    {
        using var activity = ActivitySource.StartActivity("PublishSqsFromFrontend");
        return await activity.Execute(async () =>
        {
            var messageId = await weatherApiClient.GenerateReportViaSqsAsync();
            var model = await BuildModelAsync(sqsMessageId: messageId);
            return View("~/Pages/Index.cshtml", model);
        });
    }

    private async Task<HomePageViewModel> BuildModelAsync(
        string? rabbitMqMessageId = null,
        string? sqsMessageId = null)
    {
        var forecasts = await weatherApiClient.GetForecastsAsync();
        return new HomePageViewModel
        {
            Forecasts = forecasts,
            RabbitMqMessageId = rabbitMqMessageId,
            SqsMessageId = sqsMessageId
        };
    }
}
