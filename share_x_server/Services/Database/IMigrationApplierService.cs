namespace ShareXServer.Services.Database;

public interface IMigrationApplierService
{
    Task ApplyMigrations();
}