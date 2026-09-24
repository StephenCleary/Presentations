namespace TelDemo.Frontend.Services;

public class WeatherApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<WeatherForecast>> GetForecastsAsync(CancellationToken cancellationToken = default)
    {
        var forecasts = await httpClient.GetFromJsonAsync<WeatherForecast[]>("weatherforecast", cancellationToken);
        return forecasts ?? [];
    }

    public async Task<string> PublishMessageAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync("weatherforecast/publish-message", content: null, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    public async Task<string> PublishSqsMessageAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync("weatherforecast/publish-sqs-message", content: null, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
