namespace TelDemo.Backend.Services;

public sealed class ReportGeneratorService(ILogger<ReportGeneratorService> logger)
{
    private static readonly ActivitySource ActivitySource = new("TelDemo.Backend.ReportGenerator");

    public string GenerateReport(ReadOnlyMemory<byte> body)
    {
        using var reportActivity = ActivitySource.StartActivity("GenerateReport", ActivityKind.Internal);
        reportActivity?.SetTag("report.kind", "TelemetryDemo");

        var report = $"This weather report was generated at {DateTimeOffset.UtcNow:O}";
        logger.LogInformation("Generated report for message: {Report}", report);
        return report;
    }
}
