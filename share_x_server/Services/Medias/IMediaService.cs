using FluentResults;
using ShareXServer.Database.Models;

namespace ShareXServer.Services.Medias;

public interface IMediaService
{
    Task<Result<Stream>> Get(Guid id, CancellationToken cancellationToken);
    Task<Result<Media>> Upload(Stream screenshotStream, CancellationToken cancellationToken);
    Task<Result> Delete(string deletionToken, CancellationToken cancellationToken);
}