using System;
using System.Threading.Tasks;

public class Program
{
    public static void Main()
    {
        Task[] tasks = new Task[10];

        // Создаем задачи для чтения и записи
        for (int i = 0; i < 5; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 10; j++)
                {
                    Console.WriteLine($"Read count: {Server.GetCount()}");
                }
            });
        }

        for (int i = 5; i < 10; i++)
        {
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < 5; j++)
                {
                    Server.AddToCount(1);
                    Console.WriteLine($"Added 1, new count: {Server.GetCount()}");
                }
            });
        }

        // Ожидаем завершения всех задач
        Task.WaitAll(tasks);
    }
}
