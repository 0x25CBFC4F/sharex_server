using Microsoft.Extensions.Options;
using ShareXServer.Configuration;
using ShareXServer.Database.Models;

namespace ShareXServer.Services.UrlGenerator;

public class UrlGeneratorService : IUrlGeneratorService
{
    private readonly IOptions<ServerOptions> _options;

    public UrlGeneratorService(IOptions<ServerOptions> options)
    {
        _options = options;
    }

    public (string AccessUrl, string DeleteUrl) GenerateFor(Media media)
    {
        var baseUrl = new Uri(_options.Value.BaseUrl, UriKind.Absolute);
        var viewUrl = new Uri(baseUrl, $"/media/{media.Id:N}");
        var deleteUrl = new Uri(baseUrl, $"/media/delete/{media.DeleteToken}");

        return (viewUrl.ToString(), deleteUrl.ToString());
    }

    public string GenerateFor(ShortenedUrl shortenedUrl)
    {
        return new Uri(new Uri(_options.Value.BaseUrl, UriKind.Absolute), new Uri($"/url/{shortenedUrl.Id}", UriKind.Relative)).ToString();
    }
}