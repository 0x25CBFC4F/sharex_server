using ShareXServer.Database.Models;

namespace ShareXServer.Services.UrlGenerator;

public interface IUrlGeneratorService
{
    (string AccessUrl, string DeleteUrl) GenerateFor(Media media);
    string GenerateFor(ShortenedUrl shortenedUrl);
}