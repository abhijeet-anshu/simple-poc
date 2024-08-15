using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MutexApps
{
    internal class NoMutex
    {

        static readonly int N = 1000000;

        static CountdownEvent countdown = new CountdownEvent(N);

        static int count = 0;

        static async Task IncrementCountAsync()
        {
            count++;
            countdown.Signal();
        }

        static async Task ExecuteIncrementAsync()
        {
            Parallel.For(0, N, async i =>
            {
                IncrementCountAsync();
            });
        }

        internal static void Execute()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ExecuteIncrementAsync();
            countdown.Wait();
            sw.Stop();
            Console.WriteLine($"{nameof(NoMutex)}- Final value for " + count + $", {N - count}  far from " + N + $" took  {sw.ElapsedMilliseconds} ms");
        }


    }
}
