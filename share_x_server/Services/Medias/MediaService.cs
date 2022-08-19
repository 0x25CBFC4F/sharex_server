﻿using FluentResults;
using Microsoft.Extensions.Options;
using ShareXServer.Configuration;
using ShareXServer.Database.Models;
using ShareXServer.Services.Repositories.Medias;

namespace ShareXServer.Services.Medias;

public class MediaService : IMediaService
{
    private readonly IMediaRepository _repository;
    private readonly IOptionsMonitor<ServerOptions> _optionsMonitor;
    private readonly ILogger<MediaService> _logger;

    public MediaService(IMediaRepository repository, IOptionsMonitor<ServerOptions> optionsMonitor, ILogger<MediaService> logger)
    {
        _repository = repository;
        _optionsMonitor = optionsMonitor;
        _logger = logger;
    }
    
    public async Task<Result<Stream>> Get(Guid id, CancellationToken cancellationToken)
    {
        var media = await _repository.Get(id, cancellationToken);

        if (media.IsFailed)
        {
            return Result.Fail(media.Errors.First().Message);
        }

        var fullFilePath = GetMediaPath(media.Value.FileName);
        return File.OpenRead(fullFilePath);
    }

    public async Task<Result<Media>> Upload(Stream screenshotStream, CancellationToken cancellationToken)
    {
        if (!ValidatePngHeader(screenshotStream))
        {
            return Result.Fail("Got an invalid PNG file.");
        }

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
            await screenshotStream.CopyToAsync(fileStream, cancellationToken);
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
        
        var media = await _repository.Add(fileName, cancellationToken);

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

    private static bool ValidatePngHeader(Stream stream)
    {
        var pngHeader = new byte[] { 0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a };
        var streamHeader = new byte[pngHeader.Length];
        
        stream.Seek(0, SeekOrigin.Begin);
        _ = stream.Read(streamHeader);
        stream.Seek(0, SeekOrigin.Begin);
        
        return pngHeader.SequenceEqual(streamHeader);
    }

    private string GetMediaPath(string fileName)
    {
        var options = _optionsMonitor.CurrentValue;
        return Path.Combine(options.MediaDirectory, fileName);
    }
}