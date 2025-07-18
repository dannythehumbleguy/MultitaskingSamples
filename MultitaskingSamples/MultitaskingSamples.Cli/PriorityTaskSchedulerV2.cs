namespace MultitaskingSamples.Cli;

public class PriorityTaskSchedulerV2(int maxParallelTasks)
{
    private readonly Lock _lock = new();
    private readonly SortedDictionary<int, Queue<Func<CancellationToken, Task>>> seq = new();
    private readonly SemaphoreSlim rateLimiter = new(maxParallelTasks, maxParallelTasks);
    private readonly SemaphoreSlim enqueueWaiter = new(0);
    
    public void Enqueue(Func<CancellationToken, Task> taskFactory, int priority)
    {
        lock (_lock)
        {
            if (!seq.ContainsKey(priority))
                seq[priority] = new Queue<Func<CancellationToken, Task>>();

            seq[priority].Enqueue(taskFactory);
            enqueueWaiter.Release();
        }
    }
    
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task>();
        while (!cancellationToken.IsCancellationRequested)
        {
            await enqueueWaiter.WaitAsync(cancellationToken);
            
            Func<CancellationToken, Task> taskFactory;
            lock (_lock)
            {
                var firstPriority = seq.Keys.First();
                taskFactory = seq[firstPriority].Dequeue();
                if(seq[firstPriority].Count == 0)
                    seq.Remove(firstPriority);
            }

            var task = WrapInSemaphore(taskFactory, cancellationToken);
            tasks.Add(task);
        }
        
        await Task.WhenAll(tasks);
    }

    private async Task WrapInSemaphore(Func<CancellationToken, Task> task, CancellationToken cancellationToken)
    {
        try
        {
            await rateLimiter.WaitAsync(cancellationToken);
            await task.Invoke(cancellationToken);
        }
        finally
        {
            rateLimiter.Release();
        }
    }
}