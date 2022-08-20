using Microsoft.Extensions.Options;
using ShareXServer.Configuration;
using ShareXServer.Repositories.Medias;
using ShareXServer.Services.Database;
using ShareXServer.Services.Medias;

namespace ShareXServer.HostedServices;

public class MediaCleanupService : IHostedService
{
    private readonly IMediaRepository _mediaRepository;
    private readonly IMediaService _mediaService;
    private readonly IMigrationApplierService _migrationApplierService;
    private readonly ILogger<MediaCleanupService> _logger;

    private readonly CancellationTokenSource _stopRequestedSource;
    private readonly CancellationToken _stopRequested;
    private readonly ManualResetEventSlim _stopped = new(false);
    private readonly MediaCleanupOptions _options;

    public MediaCleanupService(IMediaRepository mediaRepository, IMediaService mediaService, IMigrationApplierService migrationApplierService, IOptions<ServerOptions> options, ILogger<MediaCleanupService> logger)
    {
        _stopRequestedSource = new CancellationTokenSource();
        _stopRequested = _stopRequestedSource.Token;
        
        _mediaRepository = mediaRepository;
        _mediaService = mediaService;
        _migrationApplierService = migrationApplierService;
        
        _logger = logger;
        _options = options.Value.MediaCleanup;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogWarning("Auto media cleanup is disabled");
            return Task.CompletedTask;
        }
        
        Task.Factory.StartNew(DoMediaCleanup, TaskCreationOptions.LongRunning);
        return Task.CompletedTask;
    }

    private async Task DoMediaCleanup()
    {
        _logger.LogInformation("Waiting for migrations to apply..");
        await _migrationApplierService.WaitForMigrationsToApply();

        _logger.LogInformation("Starting media cleanup service..");

        while (!_stopRequested.IsCancellationRequested)
        {
            var outdated = await _mediaRepository.FindOutdated(_options.MaxMediaLifespan, _stopRequested);

            if (!outdated.Value.Any())
            {
                _logger.LogInformation("Found no media to cleanup. Next cleanup at ~[{CleanupTime}]", DateTime.Now.Add(_options.CheckInterval));
                try { await Task.Delay(_options.CheckInterval, _stopRequested); } catch(TaskCanceledException) { /* We don't care about this :) */ }
                continue;
            }
            
            _logger.LogInformation("Found {Count} media to cleanup", outdated.Value.Length);
            
            foreach (var media in outdated.Value)
            {
                _logger.LogInformation("Removing [{Id}] ({MediaType}, {OriginalFileName})", media.Id, media.MediaType, media.OriginalFileName);
                await _mediaService.Delete(media.DeleteToken, CancellationToken.None);
            }
            
            _logger.LogInformation("Next cleanup at ~[{CleanupTime}]", DateTime.Now.Add(_options.CheckInterval));
            try { await Task.Delay(_options.CheckInterval, _stopRequested); } catch(TaskCanceledException) { /* We don't care about this :) */ }
        }
        
        _stopped.Set();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping cleanup service gracefully..");
        
        _stopRequestedSource.Cancel();
        _stopped.Wait();

        return Task.CompletedTask;
    }
}