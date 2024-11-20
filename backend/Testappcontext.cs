namespace backend;
using Microsoft.EntityFrameworkCore;

public class Testappcontext : DbContext
{
    public DbSet<models.User>? Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(@"Server=localhost;User=root;Database=VoetbalApp", ServerVersion.Parse("8.0.21-mysql"));
    }

}
