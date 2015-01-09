using SystemUnderTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace IncorrectlyPassingUnitTest
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void MyFirstAsyncTestMethod()
        {
            var sut = new Sut();
            sut.SuccessAsync();
        }

        [TestMethod]
        public void WaitAMinuteThisIsntSupposedToPass()
        {
            var sut = new Sut();
            sut.FailAsync();
        }
        
        [TestMethod]
        public async Task OhThatsBetter()
        {
            var sut = new Sut();
            await sut.FailAsync();
        }

        [TestMethod]
        public async Task SoTheFirstOneShouldHaveBeenLikeThis()
        {
            var sut = new Sut();
            await sut.SuccessAsync();
        }
    }
}
