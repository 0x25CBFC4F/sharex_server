using FluentResults;
using Microsoft.EntityFrameworkCore;
using ShareXServer.Database;
using ShareXServer.Database.Models;

namespace ShareXServer.Services.Repositories.Screenshots;

public class ScreenshotRepository : IScreenshotRepository
{
    private readonly IDbContextFactory<RootContext> _dbContextFactory;

    public ScreenshotRepository(IDbContextFactory<RootContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Result<Screenshot>> GetScreenshot(Guid id, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var screenshot = await context.Screenshots.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
        
        return screenshot is not null ? Result.Ok(screenshot) : Result.Fail("No screenshot with that ID was found.");
    }

    public async Task<Result<Screenshot>> AddScreenshot(string fileName, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var screenshot = new Screenshot
        {
            UploadedAt = DateTime.UtcNow,
            FileName = fileName,
            DeleteToken = $"{Guid.NewGuid():N}"
        };

        await context.Screenshots.AddAsync(screenshot, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return screenshot;
    }

    public async Task<Result<Screenshot>> FindScreenshotByDeletionToken(string deletionToken, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var screenshot = await context.Screenshots.FirstOrDefaultAsync(x => x.DeleteToken.Equals(deletionToken), cancellationToken);
        
        return screenshot is not null ? Result.Ok(screenshot) : Result.Fail("No screenshot with that deletion token was found.");
    }

    public async Task<Result> DeleteScreenshot(Guid id, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var screenshot = await context.Screenshots.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);

        if (screenshot is null)
        {
            return Result.Fail("No screenshot with this ID was found.");
        }
        
        context.Screenshots.Remove(screenshot);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }

    public async Task<Result<IEnumerable<Screenshot>>> FindOutdatedScreenshots(TimeSpan maxLifespan, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var outdatedScreenshots = context.Screenshots.Where(x => DateTime.UtcNow - x.UploadedAt >= maxLifespan).AsEnumerable();

        return Result.Ok(outdatedScreenshots);
    }
}