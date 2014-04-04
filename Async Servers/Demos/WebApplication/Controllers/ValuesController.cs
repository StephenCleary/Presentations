using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApplication.Controllers
{
    public class ValuesController : ApiController
    {
        // Synchronous
        public IEnumerable<string> Get()
        {
            Thread.Sleep(1000);
            return new [] { "value1", "value2" };
        }

        // Asynchronous
        public async Task<IEnumerable<string>> Get(int id)
        {
            await Task.Delay(1000);
            return new [] { "value1", "value2" };
        }
    }
}
