using Log.Services;
using Serilog;
using Serilog.ThrowContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<WeatherForecastService>();

var logger = new LoggerConfiguration()
	.Enrich.With<ThrowContextEnricher>()
	.WriteTo.Seq(serverUrl: "http://localhost")
	.CreateLogger();
builder.Services.AddLogging(logging => logging.AddSerilog(logger));

builder.Services.AddProblemDetails();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();

app.Run();
