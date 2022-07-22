using Microsoft.EntityFrameworkCore;
using Serilog;
using ShareXServer.Configuration;
using ShareXServer.Database;
using ShareXServer.Services.Database;
using ShareXServer.Services.Repositories.Screenshots;
using ShareXServer.Services.Screenshots;
using ShareXServer.Services.Urls;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<ServerOptions>(builder.Configuration.GetSection("ShareX"));
builder.Services.AddControllers();

builder.Services.AddPooledDbContextFactory<RootContext>(x =>
{
    x.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddLogging(x =>
{
    x.ClearProviders();

    var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
        .CreateLogger();
    
    x.AddSerilog(logger);
});

builder.Services.AddScoped<IMigrationApplierService, MigrationApplierService>();
builder.Services.AddSingleton<IUrlGeneratorService, UrlGeneratorService>();
builder.Services.AddSingleton<IScreenshotService, ScreenshotService>();

builder.Services.AddSingleton<IScreenshotRepository, ScreenshotRepository>();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var migrationApplierService = scope.ServiceProvider.GetRequiredService<IMigrationApplierService>();
migrationApplierService.ApplyMigrations();

app.UseRouting();

app.UseEndpoints(x =>
{
    x.MapControllers();
});

app.Run();