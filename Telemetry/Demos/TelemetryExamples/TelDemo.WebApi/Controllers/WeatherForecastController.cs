namespace TelDemo.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(
	WeatherForecastService service,
	RabbitMqPublisherService rabbitMqPublisherService,
	SqsPublisherService sqsPublisherService) : ControllerBase
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

	[HttpPost("generate-report-via-rabbitmq")]
	public async Task<ActionResult<string>> GenerateReportViaRabbitMq()
	{
		var messageId = await rabbitMqPublisherService.PublishGenerateWeatherReportMessageAsync();
		return Ok(messageId);
	}

	[HttpPost("generate-report-via-sqs")]
	public async Task<ActionResult<string>> GenerateReportViaSqs(CancellationToken cancellationToken)
	{
		var messageId = await sqsPublisherService.PublishGenerateWeatherReportMessageAsync(cancellationToken);
		return Ok(messageId);
	}
}
