using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLConnectionPool
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    class SimpleBlockingQueueImpl
    {
        internal static void Exec()
        {
            // Create a BlockingCollection with a bounded capacity of 5
            BlockingCollection<int> blockingQueue = new BlockingCollection<int>(5);

            // Producer Task
            Task producer = Task.Run(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    blockingQueue.Add(i);
                    Console.WriteLine($"Produced: {i}");
                    Thread.Sleep(100); // Simulate work
                }
                blockingQueue.CompleteAdding();
            });

            // Consumer Task
            Task consumer = Task.Run(() =>
            {
                foreach (var item in blockingQueue.GetConsumingEnumerable())
                {
                    Console.WriteLine($"Consumed: {item}");
                    Thread.Sleep(150); // Simulate work
                }
            });

            Task.WaitAll(producer, consumer);
        }
    }

}
