namespace backend;
using Microsoft.EntityFrameworkCore;

public class Testappcontext : DbContext
{
    public DbSet<Models.User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(@"Server=localhost;User=root;Database=VoetbalApp", ServerVersion.Parse("8.0.21-mysql"));
    }

    public bool IsAdmin(string token)
    {
        using (var db = new Testappcontext())
        {
            var user = db.Users.FirstOrDefault(u => u.Token == token);
            if (user != null)
            {
                return user.IsAdmin;
            }
            return false;
        }
    }

}
