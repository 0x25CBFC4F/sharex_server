using FluentResults;
using Microsoft.EntityFrameworkCore;
using ShareXServer.Database;
using ShareXServer.Database.Enums;
using ShareXServer.Database.Models;

namespace ShareXServer.Services.Repositories.Medias;

public class MediaRepository : IMediaRepository
{
    private readonly IDbContextFactory<RootContext> _dbContextFactory;

    public MediaRepository(IDbContextFactory<RootContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Result<Media>> Get(Guid id, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var media = await context.Media.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
        return media is not null ? Result.Ok(media) : Result.Fail("No media with that ID was found.");
    }

    public async Task<Result<Media>> Add(string fileName, MediaType mediaType, string mimeType, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var media = new Media
        {
            MediaType = mediaType,
            UploadedAt = DateTime.UtcNow,
            FileName = fileName,
            MimeType = mimeType,
            DeleteToken = $"{Guid.NewGuid():N}"
        };

        await context.Media.AddAsync(media, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return media;
    }

    public async Task<Result<Media>> FindByDeletionToken(string deletionToken, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var media = await context.Media.FirstOrDefaultAsync(x => x.DeleteToken.Equals(deletionToken), cancellationToken);
        return media is not null ? Result.Ok(media) : Result.Fail("No screenshot with that deletion token was found.");
    }

    public async Task<Result> Delete(Guid id, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var media = await context.Media.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);

        if (media is null)
        {
            return Result.Fail("No media with this ID was found.");
        }
        
        context.Media.Remove(media);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Ok();
    }

    public async Task<Result<IEnumerable<Media>>> FindOutdated(TimeSpan maxLifespan, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var outdatedMedia = context.Media.Where(x => DateTime.UtcNow - x.UploadedAt >= maxLifespan).AsEnumerable();
        return Result.Ok(outdatedMedia);
    }
}