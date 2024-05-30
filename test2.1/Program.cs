using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public static class Server
{
    private static int count = 0;
    private static readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();
    private static readonly BlockingCollection<Action> actionQueue = new BlockingCollection<Action>();
    private static readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    static Server()
    {
        Task.Run(() => ProcessQueue());
    }

    public static int GetCount()
    {
        rwLock.EnterReadLock();
        try
        {
            return count;
        }
        finally
        {
            rwLock.ExitReadLock();
        }
    }

    public static void AddToCount(int value)
    {
        actionQueue.Add(() =>
        {
            rwLock.EnterWriteLock();
            try
            {
                count += value;
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        });
    }

    private static void ProcessQueue()
    {
        foreach (var action in actionQueue.GetConsumingEnumerable(cancellationTokenSource.Token))
        {
            action();
        }
    }

    public static void StopProcessing()
    {
        cancellationTokenSource.Cancel();
        actionQueue.CompleteAdding();
    }
}

public class Program
{
    public static void Main()
    {
        Task[] tasks = new Task[10];

        for (int i = 0; i < 5; i++)
        {
            tasks[i] = Task.Run(ReadCount);
        }

        for (int i = 5; i < 10; i++)
        {
            tasks[i] = Task.Run(AddToCountAndRead);
        }

        Task.WaitAll(tasks);
        Server.StopProcessing();
    }

    private static void ReadCount()
    {
        for (int j = 0; j < 10; j++)
        {
            Console.WriteLine($"Read count: {Server.GetCount()}");
        }
    }

    private static void AddToCountAndRead()
    {
        for (int j = 0; j < 5; j++)
        {
            Server.AddToCount(1);
            Console.WriteLine($"Added 1, new count: {Server.GetCount()}");
        }
    }
}
