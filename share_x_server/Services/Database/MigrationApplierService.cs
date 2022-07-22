using Microsoft.EntityFrameworkCore;
using ShareXServer.Database;

namespace ShareXServer.Services.Database;

public class MigrationApplierService : IMigrationApplierService
{
    private readonly IDbContextFactory<RootContext> _dbContextFactory;
    private readonly ILogger<MigrationApplierService> _logger;

    public MigrationApplierService(IDbContextFactory<RootContext> dbContextFactory, ILogger<MigrationApplierService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }
    
    public async Task ApplyMigrations()
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(); 
        
        var pendingMigrations = (await context.Database.GetPendingMigrationsAsync()).ToArray();

        if (!pendingMigrations.Any())
        {
            _logger.LogInformation("No migrations to apply. Database is up to date");
            return;
        }

        _logger.LogInformation("Found {Count} pending migration(-s)", pendingMigrations.Length);
        
        await context.Database.MigrateAsync();
        
        _logger.LogInformation("Migration is complete");
    } 
}