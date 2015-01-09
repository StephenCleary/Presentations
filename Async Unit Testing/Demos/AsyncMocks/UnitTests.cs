using System.Threading.Tasks;
using SystemUnderTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AsyncMocks
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public async Task MSTestDefaultBehaviorIsNotGoodForAsyncStubs()
        {
            var service = new SystemUnderTest.Fakes.StubIService();
            var sut = new Sut
            {
                Service = service
            };

            await sut.UseServiceAsync();
        }

        [TestMethod]
        public async Task ButMSTestDefaultStubBehaviorCanBeChanged()
        {
            var service = new SystemUnderTest.Fakes.StubIService
            {
                InstanceBehavior = new AsyncAwareDefaultValueStubBehavior()
            };
            var sut = new Sut
            {
                Service = service
            };

            await sut.UseServiceAsync();
        }


        [TestMethod]
        public async Task MoqWorksGreatOutOfTheBox()
        {
            var service = new Mock<IService>();
            var sut = new Sut
            {
                Service = service.Object
            };

            await sut.UseServiceAsync();
        }
    }
}
