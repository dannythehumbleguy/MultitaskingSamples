namespace MultitaskingSamples.Cli;

public class PriorityTaskSchedulerV2(int maxParallelTasks)
{
    private readonly Lock _lock = new();
    private readonly SortedDictionary<int, Queue<Func<Task>>> seq = new();
    private readonly SemaphoreSlim rateLimiter = new(maxParallelTasks, maxParallelTasks);
    private readonly SemaphoreSlim enqueueWaiter = new(0);
    
    public void Enqueue(Func<Task> taskFactory, int priority)
    {
        lock (_lock)
        {
            if (!seq.ContainsKey(priority))
                seq[priority] = new Queue<Func<Task>>();

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
            
            Func<Task> taskFactory;
            lock (_lock)
            {
                var firstPriority = seq.Keys.First();
                taskFactory = seq[firstPriority].Dequeue();
                if(seq[firstPriority].Count == 0)
                    seq.Remove(firstPriority);
            }

            var task = WrapInSemaphore(taskFactory);
            tasks.Add(task);
        }
        
        await Task.WhenAll(tasks);
    }

    private async Task WrapInSemaphore(Func<Task> task)
    {
        try
        {
            await rateLimiter.WaitAsync();
            await task.Invoke();
        }
        finally
        {
            rateLimiter.Release();
        }
    }
}