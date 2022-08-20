using Microsoft.Extensions.Options;
using ShareXServer.Configuration;
using ShareXServer.Database.Models;

namespace ShareXServer.Services.Urls;

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
}