using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRx
{
    class Program
    {
        static void Main(string[] args)
        {
            var timestamps = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Timestamp()
                .Where(x => x.Value % 2 == 0)
                .Select(x => x.Timestamp);
            // Same as:
            //   IObservable<long> intervals = Observable.Interval(TimeSpan.FromSeconds(1));
            //   IObservable<Timestamped<long>> timestampedIntervals = intervals.Timestamp();
            //   IObservable<Timestamped<long>> filteredIntervals = timestampedIntervals.Where(x => x.Value % 2 == 0);
            //   IObservable<DateTimeOffset> timestamps = filteredIntervals.Select(x => x.Timestamp);

            // Interval ==> Timestamp ==> Where filter ==> Select transformation

            Console.WriteLine("Ready...");
            Console.ReadKey();

            // Write data (and errors) to the console as they arrive.
            // The "Subscribe" travels up the chain and starts the interval timer.
            // Then data flows down the chain to the subscription, where it is written to the console.
            var subscription = timestamps
                .Subscribe(x => Console.WriteLine(x),
                    ex => Console.WriteLine(ex));

            Console.ReadKey();
        }
    }
}
