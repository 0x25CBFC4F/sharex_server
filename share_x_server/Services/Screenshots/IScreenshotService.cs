using FluentResults;
using ShareXServer.Database.Models;

namespace ShareXServer.Services.Screenshots;

public interface IScreenshotService
{
    Task<Result<Stream>> GetScreenshot(Guid id, CancellationToken cancellationToken);
    Task<Result<Screenshot>> UploadScreenshot(Stream screenshotStream, CancellationToken cancellationToken);
    Task<Result> DeleteScreenshot(string deletionToken, CancellationToken cancellationToken);
}