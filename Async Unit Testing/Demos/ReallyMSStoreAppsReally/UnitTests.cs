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
    public class E_UnitTests
    {
        [UITestMethod]
        public async Task E_A_UITestMethodDoesNotSupportAsync()
        {
            // Fails. UITestMethod doesn't support async test methods. :(
            await Task.Yield();
        }

        [TestMethod]
        public async Task E_B_HaveToUseThisWorkaround()
        {
            await MSStoreHelp.ExecuteAsync(async () => { await Task.Yield(); });
        }
    }
}
