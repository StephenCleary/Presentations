namespace TelDemo.Frontend.Services;

public class WeatherApiClient(HttpClient httpClient)
{
    public async Task<IReadOnlyList<WeatherForecast>> GetForecastsAsync(CancellationToken cancellationToken = default)
    {
        var forecasts = await httpClient.GetFromJsonAsync<WeatherForecast[]>("weatherforecast", cancellationToken);
        return forecasts ?? [];
    }
}
