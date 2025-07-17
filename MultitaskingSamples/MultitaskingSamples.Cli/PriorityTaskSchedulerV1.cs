namespace MultitaskingSamples.Cli;

public class PriorityTaskSchedulerV1(int maxParallelTasks)
{
    private readonly SortedDictionary<int, Queue<Func<Task>>> seq = new();
    private readonly SemaphoreSlim semaphore = new(maxParallelTasks, maxParallelTasks);
    
    /// <summary>
    /// Can't be invoked after RunAsync
    /// </summary>
    public void Enqueue(Func<Task> taskFactory, int priority)
    {
        if (!seq.ContainsKey(priority))
            seq[priority] = new Queue<Func<Task>>();

        seq[priority].Enqueue(taskFactory);
    }
    
    public async Task RunAsync()
    {
        var tasks = new List<Task>();
        
        foreach (var key in seq.Keys)
        {
            var queue = seq[key];
            foreach (var task in queue)
            {
                tasks.Add(WrapInSemaphore(task));
            }
            
        }
        await Task.WhenAll(tasks);
    }

    private async Task WrapInSemaphore(Func<Task> task)
    {
        try
        {
            await semaphore.WaitAsync();
            await task.Invoke();
        }
        finally
        {
            semaphore.Release();
        }
    }
}