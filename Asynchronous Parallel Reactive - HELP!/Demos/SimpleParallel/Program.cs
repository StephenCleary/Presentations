using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleParallel
{
    class Program
    {
        static void Main(string[] args)
        {
            // Warmup.
            DoWork(50);
            var stopwatch = new Stopwatch();
            var values = new[] { 25, 50, 50, 50, 30, 90, 50 };


            Console.WriteLine("Running serial...");
            Console.ReadKey();
            stopwatch.Start();

            foreach (var value in values)
                DoWork(value);
            
            stopwatch.Stop();
            Console.WriteLine("Serial time: " + stopwatch.Elapsed);


            Console.WriteLine("Running parallel...");
            Console.ReadKey();
            stopwatch.Restart();

            Parallel.ForEach(values, value => DoWork(value));

            stopwatch.Stop();
            Console.WriteLine("Parallel time: " + stopwatch.Elapsed);


            Console.ReadKey();
        }

        static void DoWork(int value)
        {
            // Represents CPU-bound work.
            for (int i = 0; i != value; ++i)
                Thread.Sleep(1);
        }
    }
}
