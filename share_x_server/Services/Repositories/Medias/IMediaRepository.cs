using FluentResults;
using ShareXServer.Database.Models;

namespace ShareXServer.Services.Repositories.Medias;

public interface IMediaRepository
{
    Task<Result<Media>> Get(Guid id, CancellationToken cancellationToken);
    Task<Result<Media>> Add(string fileName, CancellationToken cancellationToken);
    Task<Result<Media>> FindByDeletionToken(string deletionToken, CancellationToken cancellationToken);
    Task<Result> Delete(Guid id, CancellationToken cancellationToken);
    Task<Result<IEnumerable<Media>>> FindOutdated(TimeSpan maxLifespan, CancellationToken cancellationToken);
}