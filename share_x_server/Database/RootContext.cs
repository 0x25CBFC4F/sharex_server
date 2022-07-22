using Microsoft.EntityFrameworkCore;
using ShareXServer.Database.Models;

namespace ShareXServer.Database;

public class RootContext : DbContext
{
    public RootContext(DbContextOptions options) : base(options) {}
    
    public DbSet<Screenshot> Screenshots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RootContext).Assembly);
    }
}