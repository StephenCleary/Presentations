using System.Threading.Tasks;
using SystemUnderTest;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NSubstitute;
using Rhino.Mocks;

namespace AsyncMocks
{
    [TestClass]
    public class C_UnitTests
    {
        [TestMethod]
        public async Task C_A_MSTestDefaultBehaviorIsNotGoodForAsyncStubs()
        {
            var service = new SystemUnderTest.Fakes.StubIService();
            var sut = new Sut
            {
                Service = service
            };

            await sut.UseServiceAsync();
        }

        [TestMethod]
        public async Task C_B_ButMSTestDefaultStubBehaviorCanBeChanged()
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
        public async Task C_C_MoqWorksGreatOutOfTheBox()
        {
            var service = new Mock<IService>();
            var sut = new Sut
            {
                Service = service.Object
            };

            await sut.UseServiceAsync();
        }

        [TestMethod]
        public async Task C_D_FakeItEasyWorksGreatOutOfTheBox()
        {
            var service = A.Fake<IService>();
            var sut = new Sut
            {
                Service = service
            };

            await sut.UseServiceAsync();
        }

        [TestMethod]
        public async Task C_E_NSubstituteWorksGreatOutOfTheBox()
        {
            var service = Substitute.For<IService>();
            var sut = new Sut
            {
                Service = service
            };

            await sut.UseServiceAsync();
        }

        [TestMethod]
        public async Task C_F_RhinoMocksDynamicMockDoesntWorkWell()
        {
            var mocks = new Rhino.Mocks.MockRepository();
            var service = mocks.DynamicMock<IService>();
            var sut = new Sut
            {
                Service = service
            };

            mocks.ReplayAll();
            await sut.UseServiceAsync();
            mocks.VerifyAll();
        }

        [TestMethod]
        public async Task C_G_RhinoMocksButIfYouExplicitlyExpectThenOfCourseItWorks()
        {
            var mocks = new Rhino.Mocks.MockRepository();
            var service = mocks.DynamicMock<IService>();
            Expect.Call(service.DoSomethingAsync()).Return(Task.CompletedTask);
            var sut = new Sut
            {
                Service = service
            };

            mocks.ReplayAll();
            await sut.UseServiceAsync();
            mocks.VerifyAll();
        }
    }
}
