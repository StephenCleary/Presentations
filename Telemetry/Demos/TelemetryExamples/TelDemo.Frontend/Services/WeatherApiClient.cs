namespace TelDemo.Frontend.Services;

public class WeatherApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<WeatherForecast>> GetForecastsAsync()
    {
        var forecasts = await httpClient.GetFromJsonAsync<WeatherForecast[]>("weatherforecast");
        return forecasts ?? [];
    }

    public async Task<string> GenerateReportViaRabbitMqAsync()
    {
        using var response = await httpClient.PostAsync("weatherforecast/generate-report-via-rabbitmq", content: null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GenerateReportViaSqsAsync()
    {
        using var response = await httpClient.PostAsync("weatherforecast/generate-report-via-sqs", content: null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
