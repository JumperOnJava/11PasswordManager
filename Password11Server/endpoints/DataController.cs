using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql.Internal.Postgres;
using Password11.src.Util;
using Password11Lib.JsonModel;

namespace Password11Server;

[ApiController]
public class DataController : ControllerBase
{
    [Route("/api/setdata")]
    [HttpPut]
    public IActionResult SetData([FromBody] JsonUser? reqUser)
    {
        var id = Random.Shared.NextInt64();
        Console.WriteLine($"Started handling {id}");
        if (reqUser == null || string.IsNullOrEmpty(reqUser.Login) || string.IsNullOrEmpty(reqUser.PasswordHash))
        {
            Console.WriteLine($"Failed logindata {id}");
            return BadRequest("Login and password is required");
        }

        using (var db = new PasswordContext())
        {
            var accountQuery = db.Users.Where(user => user.Login == reqUser.Login)
                .Include(u => u.Tags)
                .Include(u => u.Fields)
                .Include(u => u.Accounts);
            if (!accountQuery.Any())
            {
                Console.WriteLine($"Failed account {id}");
                return Unauthorized("Account doesnt exist");
            }

            var dbUser = accountQuery.First();
            reqUser.PasswordHash = reqUser.PasswordHash.EncodeUtf8().Sha256().EncodeBase64();

            if (dbUser.PasswordHash != reqUser.PasswordHash)
            {
                return Unauthorized("Wrong password");
            }

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
                        if (t == oldid)
                            return newId;
                        return t;
                    }
                    ).ToList();
                }
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

            while (true)
            {
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
                    Console.WriteLine($"Finished ok {id}");
                    return Ok(dbUser);
                }
                catch (DbUpdateConcurrencyException)
                {
                    Console.WriteLine($"Failed - too fast {id}");
                    return new StatusCodeResult(429);
                }
            }

        }
    }

    [Route("/api/getdata")]
    [HttpGet]
    public IActionResult GetData([FromQuery(Name = "login")]string login)
    {
        using (var db = new PasswordContext())
        {
            var accountQuery = db.Users.Where(user => user.Login == login);
            if (!accountQuery.Any())
            {
                return NotFound("Account doesnt exist");
            }
            var dbUser = accountQuery
                .Include(u => u.Tags)
                .Include(u => u.Accounts)
                .Include(u => u.Fields)
                .FirstOrDefault(u => u.Login == login);
            return Ok(JsonConvert.SerializeObject(dbUser));
        }
    }
}
