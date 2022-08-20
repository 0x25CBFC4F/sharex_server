using Microsoft.EntityFrameworkCore;
using ShareXServer.Database;
using ShareXServer.Database.Models;

namespace ShareXServer.Repositories.ShortenedUrls;

public class ShortenedUrlRepository : IShortenedUrlRepository
{
    private readonly IDbContextFactory<RootContext> _dbContextFactory;

    public ShortenedUrlRepository(IDbContextFactory<RootContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    
    public async Task<string?> Get(string id, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return (await context.ShortenedUrls.SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken))?.RealUrl;
    }

    public async Task<ShortenedUrl> Add(string realUrl, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var shortenedUrl = new ShortenedUrl
        {
            Id = Guid.NewGuid().ToString("N")[..12],
            RealUrl = realUrl
        };

        await context.ShortenedUrls.AddAsync(shortenedUrl, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return shortenedUrl;
    }
}