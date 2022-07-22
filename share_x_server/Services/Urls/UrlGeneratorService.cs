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
    
    public (string AccessUrl, string DeleteUrl) GenerateFor(Screenshot screenshot)
    {
        var baseUrl = new Uri(_options.Value.PublicUrl ?? _options.Value.BaseUrl, UriKind.Absolute);
        var viewUrl = new Uri(baseUrl, $"s/{screenshot.Id:N}");
        var deleteUrl = new Uri(baseUrl, $"/s/d/{screenshot.DeleteToken}");

        return (viewUrl.ToString(), deleteUrl.ToString());
    }
}