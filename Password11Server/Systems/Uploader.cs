using Microsoft.EntityFrameworkCore;
using Password11.Datatypes;
using Password11Lib.JsonModel;

namespace Password11Server;

public class Uploader
{
    public static Uploader instance = new();
    private TaskCompletionSource tcs = new();
    private int taskNum = 0;
    public async Task WorkerInit()
    {
        while (true)
        {
            while (Queue.Any())
            {
                var task = Queue.Dequeue();
                task.Start();
                task.Wait();
                Console.WriteLine($"Finished task #{++taskNum}");
            }
            tcs = new TaskCompletionSource();
            await tcs.Task;
        }
    }
    public Uploader()
    {
        this.Queue = new Queue<Task>();
        WorkerInit();
    }

    public Queue<Task> Queue { get; set; }

    Random localRandom = new Random(DateTime.Now.Microsecond);            
    public Operation Enqueue(JsonUser reqUser)
    {
        var operation = new Operation();
        Queue.Enqueue(new Task(async () =>
        {
           
        }));
        tcs.TrySetResult();
        return operation;
    } 

}