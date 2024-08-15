using System;

namespace MutexApps
{
    internal class Program
    {
        static Task[] tasks = new Task[100];
        static void Main(string[] args)
        {
            NoMutex.Execute();

            Pessimistic.Execute();

            OptimisticVerbose.Execute();
        }
    }
}