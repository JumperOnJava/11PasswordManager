using Microsoft.EntityFrameworkCore;
using Password11Lib.JsonModel;

namespace Password11Server;

class PasswordContext : DbContext
{
    public DbSet<JsonUser> Users { get; set; }
    public DbSet<JsonAccount> Accounts { get; set; }
    public DbSet<JsonTag> Tags { get; set; }
    public DbSet<JsonField> Fields { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=pwdtest3;Username=postgres;Password=root;Search Path=public;");
    }
}