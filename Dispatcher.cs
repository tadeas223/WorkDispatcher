namespace WorkDispatcher;

using System.Collections.Concurrent;

public class Dispatcher <T> : IDisposable, IAsyncDisposable where T: IWorkerTask
{
    public int WorkerCount {get; private set;}
    public List<Worker<T>> workers = new();
    
    private BlockingCollection<T> queue = new();
    private ManualResetEventSlim queueBlock = new(false);

    private CancellationTokenSource cts = new();
    
    private object l = new();

    public Dispatcher (int workerCount)
    {
        WorkerCount = workerCount;
    }

    public void Add(T data) 
    {
        queue.Add(data);
    }

    public void Start() 
    {
        for(int i = 0; i < WorkerCount; i++) 
        {
            Worker<T> worker = new Worker<T>(this);
            workers.Add(worker);
            worker.Start(cts.Token);
        }
    }
    
    public T Take()
    {
        T data = queue.Take();
        if(queue.Count == 0) 
        {
            lock(l)
            {
                Monitor.PulseAll(l);
            }
        }
        return data;
    }

    public void WaitForEmpty()
    {
        if(queue.Count == 0)
        {
            return;
        }
        
        lock(l)
        {
            Monitor.Wait(l);
        }
    }

    public void Dispose()
    {
        cts.Cancel();
        cts.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await cts.CancelAsync();
        cts.Dispose();
    }

}
