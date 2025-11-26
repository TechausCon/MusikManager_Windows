using Microsoft.EntityFrameworkCore;
using NeuralBee.Core.Models;

namespace NeuralBee.Infrastructure.DataAccess;

public class LibraryContext : DbContext
{
    public DbSet<Track> Tracks { get; set; }
    public DbSet<Album> Albums { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // For simplicity, we're using a local SQLite database file.
        // This should be configured properly in a real application.
        optionsBuilder.UseSqlite("Data Source=NeuralBee.db");
    }
}
