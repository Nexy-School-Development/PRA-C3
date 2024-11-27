namespace backend;
using Microsoft.EntityFrameworkCore;
using backend.Models;

public class Testappcontext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Bet> Bets { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Tourney> Tourneys { get; set; }

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bet>()
            .HasOne(b => b.User)
            .WithMany(u => u.Bets)
            .HasForeignKey(b => b.Id);

        modelBuilder.Entity<Bet>()
            .HasOne(b => b.Match)
            .WithMany(m => m.Bets)
            .HasForeignKey(b => b.Id);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.HomeTeam)
            .WithMany(t => t.HomeMatches)
            .HasForeignKey(m => m.HomeTeam);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.AwayTeam)
            .WithMany(t => t.AwayMatches)
            .HasForeignKey(m => m.AwayTeam);

    }


}
