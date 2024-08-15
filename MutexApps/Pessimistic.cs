using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MutexApps
{
    internal class Pessimistic
    {

        static readonly int N = 1000000;
        static Mutex mutex = new Mutex();

        static CountdownEvent countdown = new CountdownEvent(N);

        static int count = 0;

        static void IncrementCountAsync()
        {
            mutex.WaitOne();
            count++;
            mutex.ReleaseMutex();
            countdown.Signal();
        }

        static async Task ExecuteIncrementAsync()
        {
            Parallel.For(0, N, i =>
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
            Console.WriteLine($"{nameof(Pessimistic)}- Final value for " + count + $", {N - count}  far from " + N + $" took  {sw.ElapsedMilliseconds} ms");
        }


    }
}
