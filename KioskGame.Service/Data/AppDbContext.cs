using Microsoft.EntityFrameworkCore;

namespace KioskGame.Service;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Player> Players => Set<Player>();
    public DbSet<GameSession> GameSessions => Set<GameSession>();
    public DbSet<PlayHistory> PlayHistory => Set<PlayHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>()
            .Property(p => p.Id)
            .IsRequired();

        modelBuilder.Entity<GameSession>()
        .HasKey(s => s.Id);

        modelBuilder.Entity<PlayHistory>()
        .HasKey(h => h.Id);

        base.OnModelCreating(modelBuilder);
    }
}
