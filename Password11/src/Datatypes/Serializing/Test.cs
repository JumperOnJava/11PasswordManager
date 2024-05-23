using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Password11.src.Util;

namespace Password11.Datatypes.Serializing;

public class Test
{
    private static bool testEncryption=false;

    public static void Start()
    {
        if(testEncryption){
            string key = Random.Shared.NextInt64().ToString();
            var manager = new JsonFileStorageManager(new FileByteLocation("selftest"),new StorageData());
            if (manager.Data != null)
                Console.WriteLine("Encoding test passed!");
            else
                throw new Exception("Encoding test failed");
        }

        using (var db = new PasswordContext())
        {
            var pw = new DbPassword();
            pw.Password = "1238123812";
            pw.Id = Random.Shared.NextInt64();
            db.Accounts.Add(pw);
            db.SaveChanges();
            foreach (var dbPassword in db.Accounts)
            {
                Console.WriteLine(dbPassword.Id+" "+dbPassword.Password);
            }
        }
        //Environment.Exit(0);
    }
}
class PasswordContext : DbContext
{
    public DbSet<DbPassword> Accounts { get; set; }
    public DbSet<DbTag> Tags { get; set; }
    public DbSet<DbField> Fields { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=pass11;Username=postgres;Password=root;Search Path=public;");
    }
}
public class DbPassword
{
    [Key]
    public long Id { get; set; }
    public virtual List<DbTag> Tags { get; set; }
    public virtual List<DbField> Fields { get; set; }
    [StringLength(64)]
    public string Password { get; set; }
}
public class DbTag
{
    [Key]
    public long Id { get; set; }
    [StringLength(64)]
    public string DisplayName { get; set; }
}
public class DbField
{
    [Key]
    public long Id { get; set; }
    [StringLength(64)]
    public string DisplayName { get; set; }
    [StringLength(256)]
    public string Data { get; set; }
}