using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PagingApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public async Task<IReadOnlyCollection<int>> Get(int limit = 10, int offset = 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            return Enumerable.Range(0, 23).Skip(offset).Take(limit).ToList();
        }
    }
}
