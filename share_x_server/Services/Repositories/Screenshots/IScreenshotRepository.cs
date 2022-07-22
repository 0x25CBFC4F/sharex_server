using FluentResults;
using ShareXServer.Database.Models;

namespace ShareXServer.Services.Repositories.Screenshots;

public interface IScreenshotRepository
{
    Task<Result<Screenshot>> GetScreenshot(Guid id, CancellationToken cancellationToken);
    Task<Result<Screenshot>> AddScreenshot(string fileName, CancellationToken cancellationToken);
    Task<Result<Screenshot>> FindScreenshotByDeletionToken(string deletionToken, CancellationToken cancellationToken);
    Task<Result> DeleteScreenshot(Guid id, CancellationToken cancellationToken);
    Task<Result<IEnumerable<Screenshot>>> FindOutdatedScreenshots(TimeSpan maxLifespan, CancellationToken cancellationToken);
}