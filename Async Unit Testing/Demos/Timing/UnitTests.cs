using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Shims;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _6_Timing
{
    [TestClass]
    public class F_UnitTests
    {
        [TestMethod]
        public void F_A_FakingTaskDelay()
        {
            using (ShimsContext.Create())
            {
                var scheduler = new TestScheduler();
                scheduler.FakeTaskDelay();

                var task = Task.Delay(TimeSpan.FromMinutes(2));
                Assert.IsFalse(task.IsCompleted);
                scheduler.Start();
                Assert.IsTrue(task.IsCompleted);
            }
        }

        [TestMethod]
        public void F_B_FakingCancellationTokenConstructor()
        {
            using (ShimsContext.Create())
            {
                var scheduler = new TestScheduler();
                scheduler.FakeCancellationTokenTimeouts();

                var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
                Assert.IsFalse(cts.IsCancellationRequested);
                scheduler.Start();
                Assert.IsTrue(cts.IsCancellationRequested);
            }
        }
    }
}
