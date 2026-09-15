namespace TelDemo.Backend.Services;

public sealed class ReportGeneratorService(ILogger<ReportGeneratorService> logger)
{
    private static readonly ActivitySource ActivitySource = new("TelDemo.Backend.ReportGenerator");

    public async Task<string> GenerateReportAsync(ReadOnlyMemory<byte> body)
    {
        using var reportActivity = ActivitySource.StartActivity("GenerateReport", ActivityKind.Internal);
        reportActivity?.SetTag("report.kind", "TelemetryDemo");

        var report = await reportActivity.Execute(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return $"This weather report was generated at {DateTimeOffset.UtcNow:O}";
        });
        
        logger.LogInformation("Generated report for message: {Report}", report);
        return report;
    }
}
