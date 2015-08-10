using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace xUnit
{
    public class XunitUnitTests
    {
        [Fact]
        public void SynchronousMethodHasContext()
        {
            Assert.Null(SynchronizationContext.Current);
        }

        [Fact]
        public async Task AsynchronousMethodHasContext()
        {
            Assert.Null(SynchronizationContext.Current);
            await Task.Yield();
            Assert.Null(SynchronizationContext.Current);
        }
    }
}
