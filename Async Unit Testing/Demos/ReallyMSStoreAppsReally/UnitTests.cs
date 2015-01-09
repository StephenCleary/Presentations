using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework.AppContainer;

namespace ReallyMSStoreAppsReally
{
    [TestClass]
    public class UnitTests
    {
        [UITestMethod]
        public async Task UITestMethodDoesNotSupportAsync()
        {
            // Fails. UITestMethod doesn't support async test methods. :(
            await Task.Yield();
        }

        [TestMethod]
        public async Task HaveToUseThisWorkaround()
        {
            await MSStoreHelp.ExecuteAsync(async () => { await Task.Yield(); });
        }
    }
}
