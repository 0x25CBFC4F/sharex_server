using Microsoft.EntityFrameworkCore;
using Serilog;
using ShareXServer.Configuration;
using ShareXServer.Database;
using ShareXServer.Repositories.Medias;
using ShareXServer.Repositories.ShortenedUrls;
using ShareXServer.Services.Database;
using ShareXServer.Services.Medias;
using ShareXServer.Services.UrlGenerator;
using ShareXServer.Services.UrlShortener;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<ServerOptions>(builder.Configuration.GetSection("ShareX"));
builder.Services.AddControllers();

builder.Services.AddPooledDbContextFactory<RootContext>(x => { x.UseNpgsql(builder.Configuration.GetConnectionString("Default")); });

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
builder.Services.AddSingleton<IMediaService, MediaService>();
builder.Services.AddSingleton<IUrlShortenerService, UrlShortenerService>();
builder.Services.AddSingleton<IMediaMimeTypeResolverService, MediaMimeTypeResolverService>();

builder.Services.AddSingleton<IMediaRepository, MediaRepository>();
builder.Services.AddSingleton<IShortenedUrlRepository, ShortenedUrlRepository>();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var migrationApplierService = scope.ServiceProvider.GetRequiredService<IMigrationApplierService>();
migrationApplierService.ApplyMigrations();

app.UseRouting();

app.UseEndpoints(x => { x.MapControllers(); });

app.Run();