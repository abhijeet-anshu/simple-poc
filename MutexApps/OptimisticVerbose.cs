using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace MutexApps
{
    internal class OptimisticVerbose
    {
        static readonly int N = 1000000;

        static CountdownEvent countdown = new CountdownEvent(N);

        static int count = 0;

        static async Task IncrementCountAsync()
        {

            int originalValue;
            int newValue;

            do
            {
                originalValue = count;
                newValue = originalValue + 1;
            }while (Interlocked.CompareExchange(ref count, newValue, originalValue) != originalValue);

            countdown.Signal();

            //Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} - {count}");



            
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
            Console.WriteLine($"{nameof(OptimisticVerbose)}- Final value for " + count + $", {N - count}  far from " + N + $" took  {sw.ElapsedMilliseconds} ms");
        }

    }
}
