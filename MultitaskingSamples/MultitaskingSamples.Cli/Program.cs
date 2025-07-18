using MultitaskingSamples.Cli;

var scheduler = new PriorityTaskSchedulerV2(3);

void AddTask(string name, int delayMs, int priority)
{
    scheduler.Enqueue(async () =>
    {
        Console.WriteLine($"[START] {name} (priority {priority})");
        await Task.Delay(delayMs);
        Console.WriteLine($"[END]   {name} (priority {priority})");
    }, priority);
}

// Добавим задачи с разным приоритетом
AddTask("High A",   1000, priority: 1);
AddTask("Medium A", 1500, priority: 5);
AddTask("Low A",    2000, priority: 10);

AddTask("High B",   1000, priority: 2);
AddTask("Medium B", 1500, priority: 6);
AddTask("Low B",    2000, priority: 11);

AddTask("High C",   1000, priority: 3);
AddTask("Medium C", 1500, priority: 7);
AddTask("Low C",    2000, priority: 12);

// Запускаем обработку
var source = new CancellationTokenSource();
await scheduler.RunAsync(source.Token);