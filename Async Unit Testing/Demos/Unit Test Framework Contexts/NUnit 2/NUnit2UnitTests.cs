using System;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace NUnit_2
{
    [TestFixture]
    public class NUnit2UnitTests
    {
        [Test]
        public void SynchronousMethodDoesNotHaveContext()
        {
            Assert.IsNull(SynchronizationContext.Current);
        }

        [Test]
        public async Task AsynchronousMethodDoesNotHaveContext()
        {
            Assert.IsNull(SynchronizationContext.Current);
            await Task.Yield();
            Assert.IsNull(SynchronizationContext.Current);
        }
    }
}
