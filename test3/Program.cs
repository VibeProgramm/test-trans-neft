using System;
using System.Threading;
using System.Threading.Tasks;

public class AsyncCaller
{
    private readonly EventHandler _handler;

    public AsyncCaller(EventHandler handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public bool Invoke(int timeoutMilliseconds, object sender, EventArgs e)
    {
        if (timeoutMilliseconds < 0)
            throw new ArgumentOutOfRangeException(nameof(timeoutMilliseconds), "Timeout must be non-negative.");

        using (var cts = new CancellationTokenSource())
        {
            var task = Task.Run(() =>
            {
                _handler.Invoke(sender, e);
            }, cts.Token);

            if (task.Wait(timeoutMilliseconds, cts.Token))
            {
                return task.IsCompletedSuccessfully;
            }
            else
            {
                cts.Cancel(); 
                return false; 
            }
        }
    }
}

public class Program
{
    static void MyEventHandler(object sender, EventArgs e)
    {
       
        Thread.Sleep(3000);
        Console.WriteLine("Event handler executed.");
    }

    public static void Main()
    {
        EventHandler h = new EventHandler(MyEventHandler);
        AsyncCaller ac = new AsyncCaller(h);

       
        bool completedOK = ac.Invoke(2000, null, EventArgs.Empty);
        Console.WriteLine($"Completed within 2000 ms timeout: {completedOK}");

        
        completedOK = ac.Invoke(5000, null, EventArgs.Empty);
        Console.WriteLine($"Completed within 5000 ms timeout: {completedOK}");
    }
}
