using System;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;

namespace Fixie
{
    public class FixieUnitTests
    {
        public void SynchronousMethodDoesNotHaveContext()
        {
            SynchronizationContext.Current.ShouldBe(null);
        }

        public async Task AsynchronousMethodDoesNotHaveContext()
        {
            SynchronizationContext.Current.ShouldBe(null);
            await Task.Yield();
            SynchronizationContext.Current.ShouldBe(null);
        }
    }
}
