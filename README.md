# WorkDispatcher
This library contains code for dispatching tasks between multiple threads

## How to use
Create a class that implements the IWorkerTask interface
```
class MyTask: IWorkerTask
{
  public void Execute()
  {
    // your task logic
  }
}
```
Than dispatch multiple tasks like this
```
Dispatcher<MyTask> d = new Dispatcher<MyTask>(10); // 10 workers
d.Start();

d.Add(new MyTask()) // add some tasks

d.Dispose() // dispose dispatcher
```

The dispatcher does not have any logic to wait for tasks to be finished.
