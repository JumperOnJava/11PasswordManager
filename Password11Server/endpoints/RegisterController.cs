using Microsoft.AspNetCore.Mvc;
using Password11.src.Util;
using Password11Lib.JsonModel;

namespace Password11Server;

[ApiController]
public class RegisterController : ControllerBase
{
    [Route("api/register")]
    [HttpPut]
    public IActionResult Register([FromBody] RegisterModel? model)
    {
        if (model == null || string.IsNullOrEmpty(model.Login) || string.IsNullOrEmpty(model.Password))
            return BadRequest("Login and password are required.");
        using (var db = new PasswordContext())
        {
            var count = db.Users.Count(user => user.Login == model.Login);
            if (count != 0) return Conflict("Account already exists");
            var user = new JsonUser();
            user.Login = model.Login;
            user.PasswordHash = model.Password.EncodeUtf8().Sha256().EncodeBase64();
            db.Users.Add(user);
            db.SaveChanges();
        }

        return Ok("User registered successfully.");
    }

    [Route("api/checklogin")]
    [HttpGet]
    public IActionResult Register([FromQuery(Name = "login")] string login,
        [FromQuery(Name = "password")] string password)
    {
        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            return BadRequest("Login and password is required");

        using (var db = new PasswordContext())
        {
            var accountQuery = db.Users.Where(user => user.Login == login);
            if (!accountQuery.Any())
            {
                Console.WriteLine("Failed account");
                return Unauthorized("Wrong login credentials");
            }

            var dbUser = accountQuery.First();

            if (dbUser.PasswordHash == password.EncodeUtf8().Sha256().EncodeBase64())
                return Ok();
            return Unauthorized("Wrong login credentials");
        }
    }

    [Route("api/ok")]
    [HttpGet]
    public IActionResult Ok()
    {
        return Ok("Ok 👍");
    }
}

public class RegisterModel
{
    public string Login { get; set; }
    public string Password { get; set; }
}