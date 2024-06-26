using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Password11.src.Util;
using Password11Lib.JsonModel;
using System.Text.Json;

namespace Password11Server;

[ApiController]
public class DataController : ControllerBase
{
    [Route("/api/setdata")]
    [HttpPut]
    public async Task<IActionResult> SetData([FromBody] JsonUser? reqUser)
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

            if (dbUser.PasswordHash != reqUser.PasswordHash) return Unauthorized("Wrong password");

            var result = await Uploader.instance.Enqueue(reqUser).GetResult();
            if (result.Item1)
            {
                    return Accepted();
            }
            else
            {
                return BadRequest("Failed to update data on server");
            }
        }
    }

    [Route("/api/getdata")]
    [HttpGet]
    public IActionResult GetData([FromQuery(Name = "login")] string login, [FromQuery(Name = "password")] string password)
    {
        using (var db = new PasswordContext())
        {
            var accountQuery = db.Users.Where(user => user.Login == login);
            if (!accountQuery.Any()) return Unauthorized("Wrong account credentials");

            var dbUser = accountQuery
                .Include(u => u.Tags)
                .Include(u => u.Accounts)
                .Include(u => u.Fields)
                .FirstOrDefault(u => u.Login == login);

            var PasswordHash = password.EncodeUtf8().Sha256().EncodeBase64();

            if (dbUser.PasswordHash != PasswordHash) return Unauthorized("Wrong account credentials");

            dbUser.PasswordHash = "";
            dbUser.Login = "";

            return Ok(JsonSerializer.Serialize(dbUser));
        }
    }
}