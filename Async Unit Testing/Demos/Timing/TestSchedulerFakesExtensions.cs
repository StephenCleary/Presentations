using System;
using System.Reactive.Concurrency;
using System.Threading.Fakes;
using System.Threading.Tasks.Fakes;

public static class TestSchedulerFakesExtensions
{
    public static TestTaskDelay FakeTaskDelay(this IScheduler scheduler)
    {
        var taskDelayHelper = new TestTaskDelay(scheduler);
        ShimTask.DelayInt32 = taskDelayHelper.Delay;
        ShimTask.DelayInt32CancellationToken = taskDelayHelper.Delay;
        ShimTask.DelayTimeSpan = taskDelayHelper.Delay;
        ShimTask.DelayTimeSpanCancellationToken = taskDelayHelper.Delay;
        return taskDelayHelper;
    }

    public static TestCancellationTokenSourceTimeoutHelper FakeCancellationTokenTimeouts(this IScheduler scheduler)
    {
        var cancellationTokenSourceTimeoutHelper = new TestCancellationTokenSourceTimeoutHelper(scheduler);
        ShimCancellationTokenSource.ConstructorInt32 = (@this, millisecondsDelay) =>
        {
            DefaultConstruct(@this).CancelAfter(millisecondsDelay);
        };
        ShimCancellationTokenSource.ConstructorTimeSpan = (@this, delay) =>
        {
            DefaultConstruct(@this).CancelAfter(delay);
        };
        ShimCancellationTokenSource.AllInstances.CancelAfterInt32 = cancellationTokenSourceTimeoutHelper.CancelAfter;
        ShimCancellationTokenSource.AllInstances.CancelAfterTimeSpan = cancellationTokenSourceTimeoutHelper.CancelAfter;
        return cancellationTokenSourceTimeoutHelper;
    }

    private static T DefaultConstruct<T>(T @this) where T : new()
    {
        var constructor = typeof(T).GetConstructor(Type.EmptyTypes);
        constructor.Invoke(@this, new object[0]);
        return @this;
    }
}