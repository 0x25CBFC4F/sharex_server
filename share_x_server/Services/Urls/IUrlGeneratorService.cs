using ShareXServer.Database.Models;

namespace ShareXServer.Services.Urls;

public interface IUrlGeneratorService
{
    (string AccessUrl, string DeleteUrl) GenerateFor(Media media);
}