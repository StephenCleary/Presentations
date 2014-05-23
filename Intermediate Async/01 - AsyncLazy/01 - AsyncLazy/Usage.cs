using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class Usage
{
    private readonly AsyncLazy<int> sharedResource;

    public Usage()
    {
        sharedResource = new AsyncLazy<int>(async () =>
        {
            await Task.Delay(1000);
            return 13;
        });
    }

    public async Task<int> MultiplyResource(int operand)
    {
        var resource = await sharedResource;
        return resource * operand;
    }
}