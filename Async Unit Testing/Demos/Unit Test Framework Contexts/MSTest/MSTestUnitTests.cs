using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace MSTest
{
    [TestClass]
    public class MSTestUnitTests
    {
        [TestMethod]
        public void SynchronousMethodDoesNotHaveContext()
        {
            Assert.IsNull(SynchronizationContext.Current);
        }

        [TestMethod]
        public async Task AsynchronousMethodDoesNotHaveContext()
        {
            Assert.IsNull(SynchronizationContext.Current);
            await Task.Yield();
            Assert.IsNull(SynchronizationContext.Current);
        }
    }
}
