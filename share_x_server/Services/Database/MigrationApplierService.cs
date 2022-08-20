using Microsoft.EntityFrameworkCore;
using ShareXServer.Database;

namespace ShareXServer.Services.Database;

public class MigrationApplierService : IMigrationApplierService
{
    private readonly IDbContextFactory<RootContext> _dbContextFactory;
    private readonly ILogger<MigrationApplierService> _logger;

    private static readonly object MigrationsAppliedLock = new();
    private static bool _migrationsApplied;

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
            
            lock (MigrationsAppliedLock)
            {
                _migrationsApplied = true;
            }
            
            return;
        }

        _logger.LogInformation("Found {Count} pending migration(-s)", pendingMigrations.Length);

        await context.Database.MigrateAsync();

        _logger.LogInformation("Migration is complete");
        
        lock (MigrationsAppliedLock)
        {
            _migrationsApplied = true;
        }
    }

    public async Task WaitForMigrationsToApply()
    {
        // Pretty dirty but multiple threads may access this method and *EventReset will not handle multiple threads properly
        while (true)
        {
            lock (MigrationsAppliedLock)
            {
                if (_migrationsApplied)
                {
                    return;
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}