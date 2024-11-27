namespace backend;
using Microsoft.EntityFrameworkCore;
using backend.Models;

public class Testappcontext : DbContext
{
        public DbSet<User> Users { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Bet> Bets { get; set; }
        public DbSet<Team> Teams  { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(@"Server=localhost;User=root;Database=voetbalapp", ServerVersion.Parse("8.0.21-mysql"));
    }

    public bool IsAdmin(string token)
    {
        using (var db = new Testappcontext())
        {
            var user = db.Users.FirstOrDefault(u => u.Token == token);
            if (user != null)
            {
                return user.IsAdmin ?? false;
            }
            return false;
        }
    }
}
