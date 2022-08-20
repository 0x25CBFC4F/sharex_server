using ShareXServer.Database.Models;

namespace ShareXServer.Repositories.ShortenedUrls;

public interface IShortenedUrlRepository
{
    Task<string?> Get(string id, CancellationToken cancellationToken);
    Task<ShortenedUrl> Add(string realUrl, CancellationToken cancellationToken);
}