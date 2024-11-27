namespace backend;
using Microsoft.EntityFrameworkCore;
using backend.Models;

public class Testappcontext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Bet> Bets { get; set; }
    public DbSet<Team> Teams { get; set; }

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

    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     modelBuilder.Entity<User>()
    //         .HasOne(u => u.Team)
    //         .WithMany(t => t.Users)
    //         .HasForeignKey(u => u.TeamId);

    //     modelBuilder.Entity<Team>()
    //         .HasOne(t => t.Creator)
    //         .WithMany(u => u.CreatedTeams)
    //         .HasForeignKey(t => t.CreatorId);

    //     modelBuilder.Entity<Match>()
    //         .HasOne(m => m.Team1)
    //         .WithMany(t => t.HomeMatches)
    //         .HasForeignKey(m => m.Team1Id);

    //     modelBuilder.Entity<Match>()
    //         .HasOne(m => m.Team2)
    //         .WithMany(t => t.AwayMatches)
    //         .HasForeignKey(m => m.Team2Id);

    //     modelBuilder.Entity<Match>()
    //         .HasOne(m => m.Referee)
    //         .WithMany(u => u.RefereedMatches)
    //         .HasForeignKey(m => m.RefereeId);

    //     modelBuilder.Entity<Goal>()
    //         .HasOne(g => g.Player)
    //         .WithMany(u => u.Goals)
    //         .HasForeignKey(g => g.PlayerId);

    //     modelBuilder.Entity<Goal>()
    //         .HasOne(g => g.Match)
    //         .WithMany(m => m.Goals)
    //         .HasForeignKey(g => g.MatchId);
    // }
}
