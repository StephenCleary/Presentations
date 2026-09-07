namespace TelDemo.WebApi.Middleware;

public class FakeUserAuthenticationMiddleware(RequestDelegate next, ILogger<FakeUserAuthenticationMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // [Demo 3.2]
        //using var _ = logger.BeginScope(new Dictionary<string, object?> { ["User"] = "Demo" });
        await next(context);
    }
}
