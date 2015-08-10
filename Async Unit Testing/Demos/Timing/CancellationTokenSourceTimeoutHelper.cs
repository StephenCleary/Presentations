using System;
using System.Reactive.Concurrency;
using System.Threading;

public interface ICancellationTokenSourceTimeoutHelper
{
    void CancelAfter(CancellationTokenSource source, TimeSpan delay);
    void CancelAfter(CancellationTokenSource source, int delay);
}

public class CancellationTokenSourceTimeoutHelper : ICancellationTokenSourceTimeoutHelper
{
    public void CancelAfter(CancellationTokenSource source, int delay)
    {
        source.CancelAfter(delay);
    }

    public void CancelAfter(CancellationTokenSource source, TimeSpan delay)
    {
        source.CancelAfter(delay);
    }
}

public class TestCancellationTokenSourceTimeoutHelper : ICancellationTokenSourceTimeoutHelper
{
    private readonly IScheduler _scheduler;

    public TestCancellationTokenSourceTimeoutHelper(IScheduler scheduler)
    {
        _scheduler = scheduler;
    }

    public void CancelAfter(CancellationTokenSource source, int delay)
    {
        _scheduler.Schedule(TimeSpan.FromMilliseconds(delay), () => source.Cancel());
    }

    public void CancelAfter(CancellationTokenSource source, TimeSpan delay)
    {
        _scheduler.Schedule(delay, () => source.Cancel());
    }
}