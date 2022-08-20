using FluentResults;
using ShareXServer.Database.Models;
using ShareXServer.Repositories.ShortenedUrls;

namespace ShareXServer.Services.UrlShortener;

public class UrlShortenerService : IUrlShortenerService
{
    private readonly IShortenedUrlRepository _shortenedUrlRepository;

    public UrlShortenerService(IShortenedUrlRepository shortenedUrlRepository)
    {
        _shortenedUrlRepository = shortenedUrlRepository;
    }
    
    public async Task<Result<string?>> GetRedirectUrl(string id, CancellationToken cancellationToken)
    {
        var url = await _shortenedUrlRepository.Get(id, cancellationToken);

        return url != null ?
            Result.Ok<string?>(url) :
            Result.Fail("URL does not exist.");
    }

    public async Task<Result<ShortenedUrl>> ShortenUrl(string url, CancellationToken cancellationToken)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
        {
            return Result.Fail($"Invalid URL: {url}");
        }

        var shortenedUrl = await _shortenedUrlRepository.Add(url, cancellationToken);
        return Result.Ok(shortenedUrl);
    }
}