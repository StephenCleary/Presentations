using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable ConvertToUsingDeclaration
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable ArrangeTypeModifiers
#pragma warning disable IDE0063 // Use simple 'using' statement

namespace Demos
{
    class Program
    {
        static async Task Main()
        {
            try
            {
                // Basic demo
                YieldReturnMain();
                //await AwaitAndYieldReturnMain();

                // Use case demo
                //await PagingApiMain();

                // Async LINQ demo
                //await BasicLinqMain();

                // Async LINQ Delegates demo
                //await LinqWithAsyncLambdasMain();
                //await TerminalLinqMethodMain();
                //await TerminalLinqMethodWithAsyncLambdasMain(); // (can skip)

                // LINQ -> Async LINQ demo
                //await SuperchargeLinqMain();

                // Cancellation demo
                //await SimpleCancellationMain();
                //await ComplexCancellationMain();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine("Done.");
        }

        #region IteratorBlocks
        static void YieldReturnMain()
        {
            foreach (int item in YieldReturn())
                Console.WriteLine($"Got {item}");

            // same as:
            using (IEnumerator<int> enumerator = YieldReturn().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    int item = enumerator.Current;
                    Console.WriteLine($"Got {item}");
                }
            }
        }

        static IEnumerable<int> YieldReturn()
        {
            // Deferred execution!
            yield return 1;
            yield return 2;
            yield return 3;
        }
        #endregion

        #region AwaitAndYieldReturn
        static async Task AwaitAndYieldReturnMain()
        {
            await foreach (int item in AwaitAndYieldReturn())
                Console.WriteLine($"Got {item}");

            // same as:
            await using (IAsyncEnumerator<int> enumerator = AwaitAndYieldReturn().GetAsyncEnumerator())
            {
                while (await enumerator.MoveNextAsync())
                {
                    int item = enumerator.Current;
                    Console.WriteLine($"Got {item}");
                }
            }
        }

        static async IAsyncEnumerable<int> AwaitAndYieldReturn()
        {
            await Task.Delay(TimeSpan.FromSeconds(1)); // pause (await)
            yield return 1; // pause (produce value)
            await Task.Delay(TimeSpan.FromSeconds(1)); // pause (await)
            yield return 2; // pause (produce value)
            await Task.Delay(TimeSpan.FromSeconds(1)); // pause (await)
            yield return 3; // pause (produce value)
        }
        #endregion

        #region PagingApi
        static async Task PagingApiMain()
        {
            Console.WriteLine($"{DateTime.Now:hh:mm:ss} Starting...");
            await foreach (int item in PagingApi())
                Console.WriteLine($"{DateTime.Now:hh:mm:ss} Got {item}");
        }

        static async IAsyncEnumerable<int> PagingApi()
        {
            // Handle the paging only in this function.
            // Other functions don't get polluted with paging logic.
            const int pageSize = 5;
            int offset = 0;

            while (true)
            {
                // Get next page of results.
                string jsonString = await HttpClient.GetStringAsync(
                    $"http://localhost:53198/api/values?offset={offset}&limit={pageSize}");

                // Produce them for our consumer.
                int[] results = JsonConvert.DeserializeObject<int[]>(jsonString);
                foreach (int result in results)
                    yield return result;

                // If this is the last page, then stop.
                if (results.Length != pageSize)
                    break;

                // Index to the next page.
                offset += pageSize;
            }
        }

        static readonly HttpClient HttpClient = new HttpClient();
        #endregion

        static async IAsyncEnumerable<int> SlowRange()
        {
            for (int i = 0; i != 10; ++i)
            {
                await Task.Delay(i * TimeSpan.FromSeconds(0.1));
                yield return i;
            }
        }

        #region LINQ
        static async Task BasicLinqMain()
        {
            Console.WriteLine($"{DateTime.Now:hh:mm:ss} Starting...");
            IAsyncEnumerable<int> query = SlowRange().Where(x => x % 2 == 0);
            await foreach (int item in query)
                Console.WriteLine($"{DateTime.Now:hh:mm:ss} Got {item}");
        }

        static async Task LinqWithAsyncLambdasMain()
        {
            Console.WriteLine($"{DateTime.Now:hh:mm:ss} Starting...");
            IAsyncEnumerable<int> query = SlowRange().WhereAwait(async x =>
            {
                await Task.Delay(TimeSpan.FromSeconds(0.1));
                return x % 2 == 0;
            });
            await foreach (int item in query)
                Console.WriteLine($"{DateTime.Now:hh:mm:ss} Got {item}");
        }

        static async Task TerminalLinqMethodMain()
        {
            Console.WriteLine($"{DateTime.Now:hh:mm:ss} Starting...");
            int result = await SlowRange().CountAsync(x => x % 2 == 0);
            Console.WriteLine($"{DateTime.Now:hh:mm:ss} Result: {result}");
        }

        static async Task TerminalLinqMethodWithAsyncLambdasMain()
        {
            Console.WriteLine($"{DateTime.Now:hh:mm:ss} Starting...");
            int result = await SlowRange().CountAwaitAsync(async x =>
            {
                await Task.Delay(TimeSpan.FromSeconds(0.1));
                return x % 2 == 0;
            });
            Console.WriteLine($"{DateTime.Now:hh:mm:ss} Result: {result}");
        }

        static async Task SuperchargeLinqMain()
        {
            // I have this enumerable and want to pass an async lambda to Where.
            IEnumerable<int> query = Enumerable.Range(0, 10);
            
            IAsyncEnumerable<int> asyncQuery = query.ToAsyncEnumerable()
                .WhereAwait(async x =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                    return x % 2 == 0;
                });

            Console.WriteLine($"{DateTime.Now:hh:mm:ss} Starting...");
            await foreach (int item in asyncQuery)
                Console.WriteLine($"{DateTime.Now:hh:mm:ss} Got {item}");
        }
        #endregion

        // VS reminds you of [EnumeratorCancellation]
        static async IAsyncEnumerable<int> CancelableSlowRange(
            [EnumeratorCancellation] CancellationToken token = default)
        {
            for (int i = 0; i != 10; ++i)
            {
                await Task.Delay(i * TimeSpan.FromSeconds(0.1), token);
                yield return i;
            }
        }

        #region Cancellation
        static async Task SimpleCancellationMain()
        {
            using CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            Console.WriteLine($"{DateTime.Now:hh:mm:ss} Starting...");
            await foreach (int item in CancelableSlowRange(cts.Token))
                Console.WriteLine($"{DateTime.Now:hh:mm:ss} Got {item}");
        }






        static async Task ConsumeSequenceWithTimeout(IAsyncEnumerable<int> items)
        {
            using CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            await foreach (int item in items.WithCancellation(cts.Token))
                Console.WriteLine($"{DateTime.Now:hh:mm:ss} Got {item}");
        }

        static async Task ComplexCancellationMain()
        {
            Console.WriteLine($"{DateTime.Now:hh:mm:ss} Starting...");
            await ConsumeSequenceWithTimeout(CancelableSlowRange());
        }
        #endregion
    }
}