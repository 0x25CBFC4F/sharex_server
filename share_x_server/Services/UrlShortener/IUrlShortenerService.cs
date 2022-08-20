using FluentResults;
using ShareXServer.Database.Models;

namespace ShareXServer.Services.UrlShortener;

public interface IUrlShortenerService
{
    Task<Result<string?>> GetRedirectUrl(string id, CancellationToken cancellationToken);
    Task<Result<ShortenedUrl>> ShortenUrl(string url, CancellationToken cancellationToken);
}