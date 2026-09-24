// Avoid delays reporting telemtry (only do this for demo code).
Environment.SetEnvironmentVariable("OTEL_METRIC_EXPORT_INTERVAL", "1000");
Environment.SetEnvironmentVariable("OTEL_BSP_SCHEDULE_DELAY", "1000");
Environment.SetEnvironmentVariable("OTEL_BLRP_SCHEDULE_DELAY", "1000");

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .ConfigureResource(resource => resource.AddService("TelDemo.Frontend"))
        .AddHttpClientInstrumentation()
        .AddSource("TelDemo.*"))
    .WithMetrics(metrics => metrics.ConfigureResource(resource => resource.AddService("TelDemo.Frontend")))
    .WithLogging(logging => logging.ConfigureResource(resource => resource.AddService("TelDemo.Frontend")))
    .UseOtlpExporter();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient<WeatherApiClient>(client => client.BaseAddress = new Uri("http://localhost:5250/"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapDefaultEndpoints();
app.MapControllers();
app.MapRazorPages();
app.Run();
