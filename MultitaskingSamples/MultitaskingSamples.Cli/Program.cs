using MultitaskingSamples.Cli;

var scheduler = new PriorityTaskSchedulerV1(3);

scheduler.Enqueue(async () =>
{
    Console.WriteLine("Low priority task");
    await Task.Delay(2000);
}, priority: 10);

scheduler.Enqueue(async () =>
{
    Console.WriteLine("High priority task");
    await Task.Delay(10000);
    Console.WriteLine("High priority task ends");
}, priority: 1);

scheduler.Enqueue(async () =>
{
    Console.WriteLine("Medium priority task");
    await Task.Delay(1500);
}, priority: 5);

scheduler.Enqueue(async () =>
{
    Console.WriteLine("Low priority task");
    await Task.Delay(2000);
}, priority: 12);

scheduler.Enqueue(async () =>
{
    Console.WriteLine("High priority task");
    await Task.Delay(10000);
    Console.WriteLine("High priority task ends");
}, priority: 3);

scheduler.Enqueue(async () =>
{
    Console.WriteLine("Medium priority task");
    await Task.Delay(1500);
}, priority: 6);

scheduler.Enqueue(async () =>
{
    Console.WriteLine("Low priority task");
    await Task.Delay(2000);
}, priority: 11);

scheduler.Enqueue(async () =>
{
    Console.WriteLine("High priority task");
    await Task.Delay(10000);
    Console.WriteLine("High priority task ends");
}, priority: 2);

scheduler.Enqueue(async () =>
{
    Console.WriteLine("Medium priority task");
    await Task.Delay(1500);
}, priority: 4);

await scheduler.RunAsync();