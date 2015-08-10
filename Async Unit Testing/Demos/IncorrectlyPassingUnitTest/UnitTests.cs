using SystemUnderTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace IncorrectlyPassingUnitTest
{
    [TestClass]
    public class A_UnitTests
    {
        [TestMethod]
        public void A_A_MyFirstAsyncTestMethod()
        {
            var sut = new Sut();
            sut.SuccessAsync();
        }

        [TestMethod]
        public void A_B_WaitAMinuteThisIsntSupposedToPass()
        {
            var sut = new Sut();
            sut.FailAsync();
        }
        
        [TestMethod]
        public async Task A_C_OhThatsBetter()
        {
            var sut = new Sut();
            await sut.FailAsync();
        }

        [TestMethod]
        public async Task A_D_SoTheFirstOneShouldHaveBeenLikeThis()
        {
            var sut = new Sut();
            await sut.SuccessAsync();
        }
    }
}
