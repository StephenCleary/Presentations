using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;

public interface ITaskDelay
{
    Task Delay(TimeSpan delay);
    Task Delay(TimeSpan delay, CancellationToken cancellationToken);
    Task Delay(int millisecondsDelay);
    Task Delay(int millisecondsDelay, CancellationToken cancellationToken);
}

public class TaskDelay : ITaskDelay
{
    public Task Delay(int millisecondsDelay)
    {
        return Task.Delay(millisecondsDelay);
    }

    public Task Delay(TimeSpan delay)
    {
        return Task.Delay(delay);
    }

    public Task Delay(int millisecondsDelay, CancellationToken cancellationToken)
    {
        return Task.Delay(millisecondsDelay, cancellationToken);
    }

    public Task Delay(TimeSpan delay, CancellationToken cancellationToken)
    {
        return Task.Delay(delay, cancellationToken);
    }
}

public class TestTaskDelay : ITaskDelay
{
    private readonly IScheduler _scheduler;

    public TestTaskDelay(IScheduler scheduler)
    {
        _scheduler = scheduler;
    }

    public async Task Delay(int millisecondsDelay)
    {
        await _scheduler.Sleep(TimeSpan.FromMilliseconds(millisecondsDelay)).ConfigureAwait(false);
    }

    public async Task Delay(TimeSpan delay)
    {
        await _scheduler.Sleep(delay);
    }

    public async Task Delay(int millisecondsDelay, CancellationToken cancellationToken)
    {
        await _scheduler.Sleep(TimeSpan.FromMilliseconds(millisecondsDelay), cancellationToken);
    }

    public async Task Delay(TimeSpan delay, CancellationToken cancellationToken)
    {
        await _scheduler.Sleep(delay, cancellationToken);
    }
}
