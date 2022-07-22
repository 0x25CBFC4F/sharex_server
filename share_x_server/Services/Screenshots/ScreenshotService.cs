using FluentResults;
using Microsoft.Extensions.Options;
using ShareXServer.Configuration;
using ShareXServer.Database.Models;
using ShareXServer.Services.Repositories.Screenshots;

namespace ShareXServer.Services.Screenshots;

public class ScreenshotService : IScreenshotService
{
    private readonly IScreenshotRepository _repository;
    private readonly IOptionsMonitor<ServerOptions> _optionsMonitor;
    private readonly ILogger<ScreenshotService> _logger;

    public ScreenshotService(IScreenshotRepository repository, IOptionsMonitor<ServerOptions> optionsMonitor, ILogger<ScreenshotService> logger)
    {
        _repository = repository;
        _optionsMonitor = optionsMonitor;
        _logger = logger;
    }
    
    public async Task<Result<Stream>> GetScreenshot(Guid id, CancellationToken cancellationToken)
    {
        var screenshot = await _repository.GetScreenshot(id, cancellationToken);

        if (screenshot.IsFailed)
        {
            return Result.Fail(screenshot.Errors.First().Message);
        }

        var fullFilePath = GetScreenshotPath(screenshot.Value.FileName);
        return File.OpenRead(fullFilePath);
    }

    public async Task<Result<Screenshot>> UploadScreenshot(Stream screenshotStream, CancellationToken cancellationToken)
    {
        if (!ValidatePngHeader(screenshotStream))
        {
            return Result.Fail("Got an invalid PNG file.");
        }

        var options = _optionsMonitor.CurrentValue;
        
        var fileName = $"{Guid.NewGuid():N}.png";

        Directory.CreateDirectory(options.ScreenshotsDirectory);

        var fullFilePath = GetScreenshotPath(fileName);
        
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
        
        var screenshot = await _repository.AddScreenshot(fileName, cancellationToken);

        if (screenshot.IsFailed)
        {
            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
            }
            
            _logger.LogError("Failed to create screenshot entry in the database");
            return Result.Fail("Internal error.");
        }

        return screenshot;
    }

    public async Task<Result> DeleteScreenshot(string deletionToken, CancellationToken cancellationToken)
    {
        var screenshotResult = await _repository.FindScreenshotByDeletionToken(deletionToken, cancellationToken);

        if (screenshotResult.IsFailed)
        {
            return Result.Fail("Unknown deletion token.");
        }

        var screenshot = screenshotResult.Value;
        var screenshotPath = GetScreenshotPath(screenshot.FileName);

        try
        {
            if (File.Exists(screenshotPath))
            {
                File.Delete(screenshotPath);
            }
        }
        finally
        {
            await _repository.DeleteScreenshot(screenshot.Id, cancellationToken);
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

    private string GetScreenshotPath(string fileName)
    {
        var options = _optionsMonitor.CurrentValue;
        return Path.Combine(options.ScreenshotsDirectory, fileName);
    }
}