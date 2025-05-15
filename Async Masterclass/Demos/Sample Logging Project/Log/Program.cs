using Nito.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Custom
builder.Logging.AddSimpleConsole(x => x.IncludeScopes = true);
builder.Services.AddExceptionLoggingScopes();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

// Custom
//app.Use(async (context, next) =>
//{
//	var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
//		.CreateLogger("DemoUserMiddleware");
//	using var loggerScope = logger.BeginDataScope(("User", "Demo"));
//	await next(context);
//});

app.MapControllers();

app.Run();
