using Nito.AsyncEx;
using System.Net.Http;

namespace WpfApp1;

public class MoreExamples
{
    #region async void
    public static async void ExampleAsyncVoid()
    {
        await Task.Delay(1000);
    }

    public static async Task ExampleAsyncTask()
    {
        await Task.Delay(1000);
    }

    public static void Caller()
    {
        ExampleAsyncVoid();
        // No way to await or catch exceptions
    }
    #endregion

    #region Blocking on asynchronous code
    public static void BlockingCaller()
    {
        // Same problem with Wait() or Result.
        ExampleAsyncTask().GetAwaiter().GetResult();
    }
    #endregion

    #region Fire and forget
    public static void FireAndForget()
    {
        _ = ExampleAsyncTask();
        // Task is discarded - ignored!
    }
    #endregion

    #region Dangerous Task methods
    public static void ContinueWithIsDangerous()
    {
        Task task = ExampleAsyncTask();
        task.ContinueWith(t =>
        {
            Console.WriteLine("Completed");
        });
    }

    public static void StartNewIsDangerous()
    {
        Task task = Task.Factory.StartNew(() =>
        {
            Console.WriteLine("Running");
        });
    }

    public static void TaskConstructorHasZeroUseCases()
    {
        Task task = new Task(() =>
        {
            Console.WriteLine("Running");
        });
        task.Start();
    }
    #endregion

    #region Side effects
    private static int _value = 0;
    public static async Task SideEffectsAsync(int x)
    {
        await Task.Delay(1000);
        _value = x * 2;
    }

    public static async Task CallerWithSideEffects()
    {
        var task = SideEffectsAsync(21);
        // What is the value of _value here?
        await task;
        Console.WriteLine(_value); // 42
    }
    #endregion

    #region Eliding
    public static async Task DoSomethingAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
    }

    public static async Task DoSomethingAsync() =>
        await DoSomethingAsync(CancellationToken.None);

    public static Task ElidingWithUsing()
    {
        using var cts = new CancellationTokenSource();
        return DoSomethingAsync(cts.Token);
    }

    public static Task ElidingWithTryCatch()
    {
        try
        {
            return DoSomethingAsync();
        }
        catch
        {
            // Handle exceptions
            throw;
        }
    }
    #endregion

    #region WaitAsync to cancel
    public static async Task WaitAsyncExample(CancellationToken cancellationToken)
    {
        await Task.Delay(1000).WaitAsync(cancellationToken);
    }
    #endregion

    #region Fake async
    public static Task FakeAsync()
    {
        return Task.Run(() =>
        {
            Thread.Sleep(1000);
        });
    }
    #endregion

    #region Processing tasks as they complete
    public static async Task DumpAsTheyComplete(List<Task<string>> tasks)
    {
        while (tasks.Count > 0)
        {
            Task<string> completedTask = await Task.WhenAny(tasks);
            tasks.Remove(completedTask);
            Console.WriteLine(await completedTask);
        }
    }

    // If you must: Task.WhenEach is in .NET 9
    public static async Task DumpAsTheyCompleteWithWhenEach(List<Task<string>> tasks)
    {
        await foreach (Task<string> completedTask in Task.WhenEach(tasks))
        {
            Console.WriteLine(await completedTask);
        }
    }

    // Or AsyncEx has OrderByCompletion for all supported .NET versions
    public static async Task DumpAsTheyCompleteWithAsyncEx(List<Task<string>> tasks)
    {
        foreach (Task<string> completedTask in tasks.OrderByCompletion())
        {
            Console.WriteLine(await completedTask);
        }
    }
    #endregion

    #region Unnecessary callbacks
    // More common in Node.js
    public static async Task UnnecessaryCallbacks(Action<string> onResult, Action<Exception> onError)
    {
        using var client = new HttpClient();
        try
        {
            string result = await client.GetStringAsync("https://www.google.com");
            onResult(result);
        }
        catch (Exception ex)
        {
            onError(ex);
        }
    }
    #endregion
}
