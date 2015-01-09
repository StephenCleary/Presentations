using System;
using System.Threading.Tasks;
using SystemUnderTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ABitMoreRealWorld
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public async Task UseService_ServiceSucceedsAsync_Succeeds()
        {
            var service = new Mock<IService>();
            service.Setup(x => x.DoSomethingAsync()).Returns(async () =>
            {
                // Helper method would be nice.
                await Task.Yield();
            });
            var sut = new Sut
            {
                Service = service.Object
            };

            await sut.UseServiceAsync();
        }

        [TestMethod]
        public async Task UseService_ServiceSucceedsSync_Succeeds()
        {
            var service = new Mock<IService>();
            service.Setup(x => x.DoSomethingAsync()).Returns(() => Task.FromResult(0));
            var sut = new Sut
            {
                Service = service.Object
            };

            await sut.UseServiceAsync();
        }

        [TestMethod]
        public async Task UseService_ServiceFailsAync_PropagatesException()
        {
            var service = new Mock<IService>();
            var exception = new Exception("Test failure.");
            service.Setup(x => x.DoSomethingAsync()).Callback(async () =>
            {
                // Again, a helper method would be better.
                await Task.Yield();
                throw exception;
            });
            var sut = new Sut
            {
                Service = service.Object
            };

            var resultException = await AsyncAssert.ThrowsAsync<Exception>(() => sut.UseServiceAsync());

            Assert.AreSame(exception, resultException);
        }
    }
}
