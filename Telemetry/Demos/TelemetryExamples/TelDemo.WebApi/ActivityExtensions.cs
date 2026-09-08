#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace TelDemo;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ActivityExtensions
{
    public static void Execute(this Activity? activity, Action action)
    {
        try
        {
            action();
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }

    public static TResult Execute<TResult>(this Activity? activity, Func<TResult> func)
    {
        try
        {
            var result = func();
            activity?.SetStatus(ActivityStatusCode.Ok);
            return result;
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }

    public static async Task Execute(this Activity? activity, Func<Task> func)
    {
        try
        {
            await func();
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }

    public static async Task<TResult> Execute<TResult>(this Activity? activity, Func<Task<TResult>> func)
    {
        try
        {
            var result = await func();
            activity?.SetStatus(ActivityStatusCode.Ok);
            return result;
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            throw;
        }
    }
}
