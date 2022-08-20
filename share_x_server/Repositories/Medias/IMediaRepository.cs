using FluentResults;
using ShareXServer.Database.Enums;
using ShareXServer.Database.Models;

namespace ShareXServer.Repositories.Medias;

public interface IMediaRepository
{
    Task<Result<Media>> Get(Guid id, CancellationToken cancellationToken);
    Task<Result<Media>> Add(string fileName, string originalFileName, MediaType mediaType, string mimeType, CancellationToken cancellationToken);
    Task<Result<Media>> FindByDeletionToken(string deletionToken, CancellationToken cancellationToken);
    Task<Result> Delete(Guid id, CancellationToken cancellationToken);
    Task<Result<Media[]>> FindOutdated(TimeSpan maxLifespan, CancellationToken cancellationToken);
}