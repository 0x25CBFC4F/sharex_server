using FluentResults;
using ShareXServer.Database.Models;

namespace ShareXServer.Services.Medias;

public interface IMediaService
{
    Task<Result<MediaInfo>> Get(Guid id, CancellationToken cancellationToken);
    Task<Result<Media>> Upload(string originalFileName, Stream mediaStream, bool isText, CancellationToken cancellationToken);
    Task<Result> Delete(string deletionToken, CancellationToken cancellationToken);
}