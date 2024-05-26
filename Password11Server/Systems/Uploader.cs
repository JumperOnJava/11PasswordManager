using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Password11.Datatypes;
using Password11Lib.JsonModel;

namespace Password11Server;

public class Uploader
{
    public static Uploader instance = new();

    private Random localRandom = new(DateTime.Now.Microsecond);
    private int taskNum;
    private TaskCompletionSource tcs = new();

    public Uploader()
    {
        Queue = new Queue<Task>();
        WorkerInit();
    }

    public Queue<Task> Queue { get; set; }

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

    public Operation<IActionResult> Enqueue(JsonUser reqUser)
    {
        var operation = new EmptyOperation<IActionResult>();
        Queue.Enqueue(new Task(async () =>
        {
            using (var db = new PasswordContext())
            {
                var accountQuery = db.Users.Where(user => user.Login == reqUser.Login)
                    .Include(u => u.Tags)
                    .Include(u => u.Fields)
                    .Include(u => u.Accounts);
                if (!accountQuery.Any())
                {
                    Console.WriteLine("Failed account");
                    operation.FinishSuccess(new UnauthorizedObjectResult("Account doesnt exist"));
                    return;
                }

                var dbUser = accountQuery.First();
                foreach (var tag in reqUser.Tags)
                {
                    var oldid = tag.Id;
                    var newId = Random.Shared.NextInt64();
                    tag.Id = newId;
                    db.Tags.Add(tag);
                    foreach (var reqAccount in reqUser.Accounts)
                        reqAccount.Tags = reqAccount.Tags.Select(t =>
                            {
                                if (t == oldid)
                                    return newId;
                                return t;
                            }
                        ).ToList();
                }


                foreach (var reqAccount in reqUser.Accounts)
                {
                    reqAccount.Fields = reqAccount.Fields.Select(id =>
                    {
                        var newId = Random.Shared.NextInt64();
                        reqUser.Fields.First(f => f.Id == id).Id = newId;
                        return newId;
                    }).ToList();
                    reqAccount.Id = Random.Shared.NextInt64();
                }

                foreach (var reqAccount in reqUser.Accounts)
                {
                    reqUser.Fields.ForEach(e => db.Fields.Add(e));
                    db.Accounts.Add(reqAccount);
                }

                try
                {
                    accountQuery = db.Users.Where(user => user.Login == reqUser.Login)
                        .Include(u => u.Tags)
                        .Include(u => u.Fields)
                        .Include(u => u.Accounts);
                    dbUser = accountQuery.First();
                    db.Fields.RemoveRange(dbUser.Fields.ToList());
                    db.Accounts.RemoveRange(dbUser.Accounts.ToList());
                    db.Tags.RemoveRange(dbUser.Tags.ToList());
                    dbUser.Tags.AddRange(reqUser.Tags);
                    dbUser.Fields.AddRange(reqUser.Fields);
                    dbUser.Accounts.AddRange(reqUser.Accounts);

                    db.Update(dbUser);
                    db.SaveChanges();
                    Console.WriteLine("Finished ok");
                    operation.FinishSuccess(new OkObjectResult(dbUser));
                }
                catch (DbUpdateConcurrencyException)
                {
                    Console.WriteLine("Failed - too fast");
                    operation.FinishSuccess(new StatusCodeResult(429));
                }
            }
        }));
        tcs.TrySetResult();
        return operation;
    }
}