using FluentResults;
using Microsoft.Extensions.Options;
using ShareXServer.Configuration;
using ShareXServer.Database.Models;
using ShareXServer.Repositories.Medias;

namespace ShareXServer.Services.Medias;

public class MediaService : IMediaService
{
    private readonly ILogger<MediaService> _logger;
    private readonly IMediaMimeTypeResolverService _mimeTypeResolverService;
    private readonly IOptionsMonitor<ServerOptions> _optionsMonitor;
    private readonly IMediaRepository _repository;

    public MediaService(IMediaRepository repository, IOptionsMonitor<ServerOptions> optionsMonitor, ILogger<MediaService> logger, IMediaMimeTypeResolverService mimeTypeResolverService)
    {
        _repository = repository;
        _optionsMonitor = optionsMonitor;
        _logger = logger;
        _mimeTypeResolverService = mimeTypeResolverService;
    }

    public async Task<Result<MediaInfo>> Get(Guid id, CancellationToken cancellationToken)
    {
        var mediaResult = await _repository.Get(id, cancellationToken);

        if (mediaResult.IsFailed)
        {
            return Result.Fail(mediaResult.Errors.First().Message);
        }

        var media = mediaResult.Value;
        var fullFilePath = GetMediaPath(media.FileName);

        return Result.Ok(new MediaInfo(media.OriginalFileName, media.MediaType, File.OpenRead(fullFilePath), media.MimeType));
    }

    public async Task<Result<Media>> Upload(string originalFileName, Stream mediaStream, bool isText, CancellationToken cancellationToken)
    {
        var (mediaType, mediaMimeType) = _mimeTypeResolverService.Resolve(mediaStream, isText);
        var options = _optionsMonitor.CurrentValue;
        var fileName = $"{Guid.NewGuid():N}.bin";

        Directory.CreateDirectory(options.MediaDirectory);

        var fullFilePath = GetMediaPath(fileName);

        FileStream? fileStream;

        try
        {
            fileStream = File.Create(fullFilePath);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to create file on the disk");
            return Result.Fail("Failed to create file on the disk.");
        }

        try
        {
            await mediaStream.CopyToAsync(fileStream, cancellationToken);
            await fileStream.FlushAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            fileStream.Close();
            File.Delete(fullFilePath);
            _logger.LogError(exception, "Failed to write a file to the disk");
            return Result.Fail("Failed to write a file to the disk.");
        }

        fileStream.Close();

        var media = await _repository.Add(fileName, originalFileName, mediaType, mediaMimeType, cancellationToken);

        if (media.IsFailed)
        {
            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
            }

            _logger.LogError("Failed to create media entry in the database");
            return Result.Fail("Internal error.");
        }

        return media;
    }

    public async Task<Result> Delete(string deletionToken, CancellationToken cancellationToken)
    {
        var screenshotResult = await _repository.FindByDeletionToken(deletionToken, cancellationToken);

        if (screenshotResult.IsFailed)
        {
            return Result.Fail("Unknown deletion token.");
        }

        var screenshot = screenshotResult.Value;
        var screenshotPath = GetMediaPath(screenshot.FileName);

        try
        {
            if (File.Exists(screenshotPath))
            {
                File.Delete(screenshotPath);
            }
        }
        finally
        {
            await _repository.Delete(screenshot.Id, cancellationToken);
        }

        return Result.Ok();
    }

    private string GetMediaPath(string fileName)
    {
        var options = _optionsMonitor.CurrentValue;
        return Path.Combine(options.MediaDirectory, fileName);
    }
}