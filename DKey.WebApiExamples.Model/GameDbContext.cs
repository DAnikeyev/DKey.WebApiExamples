using Microsoft.EntityFrameworkCore;

namespace DKey.WebApiExamples.Model;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options)
        : base(options)
    {
    }

    public DbSet<Game> Games { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Score).IsRequired();
            entity.Property(e => e.Comment).IsRequired(false);

            // Add indexes
            entity.HasIndex(e => e.Name).HasDatabaseName("IX_Games_Name");
        });
    }
}