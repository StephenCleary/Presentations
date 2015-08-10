using System;
using System.Threading.Tasks;
using SystemUnderTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExceptionalSituations
{
    [TestClass]
    public class B_UnitTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task B_A_OldSchoolExceptionAttributeWorks()
        {
            var sut = new Sut();
            await sut.FailAsync();
        }

        [TestMethod]
        public async Task B_B_NewStyleRequiresABitOfHelpForNow()
        {
            var sut = new Sut();
            await AsyncAssert.ThrowsAsync<Exception>(() => sut.FailAsync());
        }
    }
}
