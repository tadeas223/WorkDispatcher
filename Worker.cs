namespace WorkDispatcher;

public class Worker<T> where T: IWorkerTask
{
    private Task? workTask;
    
    private Dispatcher<T> dispatcher;

    public Worker(Dispatcher<T> dispatcher)
    {
        this.dispatcher = dispatcher;
    }

    public void Start(CancellationToken cancellationToken = default) {
        workTask = Task.Run(() => {
            while(!cancellationToken.IsCancellationRequested)
            {
                T data = dispatcher.Take();
                data.Execute();
            }
        });
    }
}
