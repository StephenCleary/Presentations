using System;
using System.Threading.Tasks;
using SystemUnderTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;

namespace ABitMoreRealWorld
{
    [TestClass]
    public class D_UnitTests
    {
        [TestMethod]
        public async Task D_A_UseService_ServiceSucceeds_Succeeds()
        {
            var service = new Mock<IService>();
            var sut = new Sut
            {
                Service = service.Object
            };

            await sut.UseServiceAsync();
        }

        [TestMethod]
        public async Task D_B_UseService_ServiceFails_PropagatesException()
        {
            var service = new Mock<IService>();
            var exception = new Exception("Test failure.");
            service.Setup(x => x.DoSomethingAsync()).Returns(Task.FromException(exception));
            var sut = new Sut
            {
                Service = service.Object
            };

            var resultException = await AsyncAssert.ThrowsAsync<Exception>(async () => await sut.UseServiceAsync());

            Assert.AreSame(exception, resultException);
        }
    }
}
