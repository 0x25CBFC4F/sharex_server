using ShareXServer.Database.Enums;

namespace ShareXServer.Services.Medias;

/// <summary>
/// Media Mime-type resolver service
/// </summary>
public interface IMediaMimeTypeResolverService
{
    /// <summary>
    /// Attempts to resolve correct mime-type for suggested type and checks header bytes for additional validation.
    /// </summary>
    /// <param name="stream"><see cref="Stream"/> containing media</param>
    /// <param name="isText"></param>
    /// <returns>File mime-type</returns>
    (MediaType mediaType, string MimeType) Resolve(Stream stream, bool isText);
}