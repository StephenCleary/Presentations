namespace TelDemo.Frontend.Services;

public class WeatherApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<WeatherForecast>> GetForecastsAsync(CancellationToken cancellationToken = default)
    {
        var forecasts = await httpClient.GetFromJsonAsync<WeatherForecast[]>("weatherforecast", cancellationToken);
        return forecasts ?? [];
    }

    public async Task<string> GenerateReportViaRabbitMqAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync("weatherforecast/generate-report-via-rabbitmq", content: null, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public async Task<string> GenerateReportViaSqsAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync("weatherforecast/generate-report-via-sqs", content: null, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
