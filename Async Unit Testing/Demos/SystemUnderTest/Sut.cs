using System;
using System.Threading.Tasks;

namespace SystemUnderTest
{
    public sealed partial class Sut
    {
        public async Task SuccessAsync()
        {
            await Task.Delay(10);
        }

        public async Task FailAsync()
        {
            await Task.Delay(10);

            // This method has low self-esteem.
            throw new Exception("I'm a failure!");
        }
    }
}
