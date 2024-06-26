using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Password11.Datatypes;
using Password11Lib.JsonModel;
using System.Collections.Concurrent;

namespace Password11Server;

public class Uploader
{
    public static Uploader instance = new();
    private int taskNum = 0;

    public Uploader()
    {
        this.Queue = new ConcurrentQueue<Task>();
    }

    public ConcurrentQueue<Task> Queue { get; set; }

    public Operation<IActionResult> Enqueue(JsonUser reqUser)
    {
        var operation = new Operation<IActionResult>();

        void Action()
        {
            using var db = new PasswordContext();
            var accountQuery = db.Users.Where(user => user.Login == reqUser.Login)
                .Include(u => u.Tags)
                .Include(u => u.Fields)
                .Include(u => u.Accounts);
            if (!accountQuery.Any())
            {
                Console.WriteLine($"Failed account");
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
                {
                    reqAccount.Tags = reqAccount.Tags.Select(t =>
                        {
                            if (t == oldid) return newId;
                            return t;
                        })
                        .ToList();
                }
            }


            foreach (var reqAccount in reqUser.Accounts)
            {
                reqAccount.Fields = reqAccount.Fields.Select(id =>
                    {
                        var newId = Random.Shared.NextInt64();
                        reqUser.Fields.First(f => f.Id == id).Id = newId;
                        return newId;
                    })
                    .ToList();
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
                Console.WriteLine($"Finished ok");
                operation.FinishSuccess(new OkObjectResult(dbUser));
                return;
            }
            catch (DbUpdateConcurrencyException)
            {
                Console.WriteLine($"Failed - too fast");
                Action();
                //operation.FinishSuccess(new StatusCodeResult(429));
            }
        }

        if (Queue.Any())
        {
            Queue.Enqueue(new Task(Action));
        }
        else
        {
            Queue.Enqueue(new Task(Action));
            while (Queue.TryDequeue(out var task))
            {
                if(task!=null)
                    task.Start();
                Console.WriteLine($"Finished task #{++taskNum}");
            }
        }
        return operation;
    }
}