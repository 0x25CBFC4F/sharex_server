using ShareXServer.Database.Enums;

namespace ShareXServer.Services.Medias;

public record MediaInfo(string OriginalFileName, MediaType MediaType, Stream Stream, string MimeType);