using System;
using System.Threading.Tasks;
using SystemUnderTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExceptionalSituations
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task OldSchoolExceptionAttributeWorks()
        {
            var sut = new Sut();
            await sut.FailAsync();
        }

        [TestMethod]
        public async Task NewStyleRequiresABitOfHelpForNow()
        {
            var sut = new Sut();
            await AsyncAssert.ThrowsAsync<Exception>(() => sut.FailAsync());
        }
    }
}
