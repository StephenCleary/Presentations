using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace SimpleDataflow
{
    class Program
    {
        static void Main(string[] args)
        {
            var multiplyBlock = new TransformBlock<int, int>(value => value * 2);
            var subtractBlock = new TransformBlock<int, int>(value => value - 2);
            var displayBlock = new ActionBlock<int>(value => Console.WriteLine(value));

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            multiplyBlock.LinkTo(subtractBlock, linkOptions);
            subtractBlock.LinkTo(displayBlock, linkOptions);

            foreach (var i in Enumerable.Range(0, 10))
                multiplyBlock.Post(i);
            multiplyBlock.Complete();

            displayBlock.Completion.Wait();

            Console.ReadKey();
        }
    }
}
