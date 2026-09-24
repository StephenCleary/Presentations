// Avoid delays reporting telemtry (only do this for demo code).
Environment.SetEnvironmentVariable("OTEL_METRIC_EXPORT_INTERVAL", "1000");
Environment.SetEnvironmentVariable("OTEL_BSP_SCHEDULE_DELAY", "1000");
Environment.SetEnvironmentVariable("OTEL_BLRP_SCHEDULE_DELAY", "1000");

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddAWSInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource("TelDemo.*")
        .AddSource("RabbitMQ.*"))
    .UseOtlpExporter();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSingleton<RabbitMqPublisherService>();
builder.Services.AddSingleton<SqsPublisherService>();
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
