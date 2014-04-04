using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main()
        {
            try
            {
                MainAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.ReadKey();
        }

        static async Task MainAsync()
        {
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;

            int connections;
            var syncUrl = "http://localhost:19473/api/values/";
            var asyncUrl = "http://localhost:19473/api/values/13";

            connections = Environment.ProcessorCount;
            Console.WriteLine(" Synchronous time for " + connections + " connections: " +
                await RunTest(syncUrl, connections));

            connections = Environment.ProcessorCount + 1;
            Console.WriteLine(" Synchronous time for " + connections + " connections: " +
                await RunTest(syncUrl, connections));

            connections = Environment.ProcessorCount;
            Console.WriteLine("Asynchronous time for " + connections + " connections: " +
                await RunTest(asyncUrl, connections));

            connections = Environment.ProcessorCount + 1;
            Console.WriteLine("Asynchronous time for " + connections + " connections: " +
                await RunTest(asyncUrl, connections));
        }

        static async Task<TimeSpan> RunTest(string url, int concurrentConnections)
        {
            var sw = new Stopwatch();
            var client = new HttpClient();

            await client.GetStringAsync(url); // warmup
            sw.Start();
            await Task.WhenAll(Enumerable.Range(0, concurrentConnections).Select(i => client.GetStringAsync(url)));
            sw.Stop();

            return sw.Elapsed;
        }
    }
}
